using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Auth;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DigitalPortal.DTOs;
using Khedmetak.DigitalPortal.Services.Abstraction;
using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Entities.Khedmetak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DigitalPortalController : ControllerBase
    {
        private readonly IDigitalPortalService _portalService;
        private readonly UserManager<User> _userManager;
        private readonly JwtService _jwtService;
        private readonly AppDbContext _context;

        public DigitalPortalController(
            IDigitalPortalService portalService,
            UserManager<User> userManager,
            JwtService jwtService,
            AppDbContext context)
        {
            _portalService = portalService;
            _userManager = userManager;
            _jwtService = jwtService;
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost("send-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> SendOtp([FromBody] DigitalPortalLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NationalId) || string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                return BadRequest(ApiResponse<string>.Fail("الرقم القومي ورقم الهاتف مطلوبان"));
            }

            var result = await _portalService.SendOtpAsync(dto);
            if (!result)
            {
                return BadRequest(ApiResponse<string>.Fail("البيانات المدخلة غير صحيحة أو غير مسجلة في بوابة مصر الرقمية"));
            }

            return Ok(ApiResponse<string>.Ok("تم إرسال كود التحقق (123456) إلى هاتفك بنجاح (للتجربة حالياً استخدم 123456)"));
        }

        [HttpPost("verify-otp-login")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtpLogin([FromBody] DigitalPortalOtpDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NationalId) || string.IsNullOrWhiteSpace(dto.Otp))
            {
                return BadRequest(ApiResponse<string>.Fail("الرقم القومي وكود التحقق مطلوبان"));
            }

            var citizen = await _portalService.VerifyOtpAndGetCitizenAsync(dto);
            if (citizen == null)
            {
                return BadRequest(ApiResponse<string>.Fail("كود التحقق غير صحيح أو غير متطابق"));
            }

            // 1. Check if user already exists via CitizenProfile.NationalId
            var profile = await _context.CitizenProfiles
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.NationalId == citizen.NationalId);

            User user;
            if (profile != null)
            {
                user = profile.User;
            }
            else
            {
                // 2. User does not exist, auto-provision user and citizen profile
                var generatedEmail = $"{citizen.NationalId}@digitalportal.gov.eg";
                
                // Double check if email is somehow in use
                var existingUserByEmail = await _userManager.FindByEmailAsync(generatedEmail);
                if (existingUserByEmail != null)
                {
                    user = existingUserByEmail;
                }
                else
                {
                    user = new User
                    {
                        UserName = generatedEmail,
                        Email = generatedEmail,
                        Name = citizen.FullName,
                        PhoneNumber = citizen.PhoneNumber,
                        Role = "User",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.UtcNow,
                        CitizenProfile = new CitizenProfile
                        {
                            FullName = citizen.FullName,
                            NationalId = citizen.NationalId,
                            IsVerifiedViaDigitalPortal = true,
                            DateOfBirth = citizen.DateOfBirth,
                            City = citizen.City,
                            District = citizen.District,
                            Street = citizen.Street,
                            BuildingNumber = citizen.BuildingNumber,
                            FloorNumber = citizen.FloorNumber,
                            ApartmentNumber = citizen.ApartmentNumber,
                            PostalCode = citizen.PostalCode,
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    var createResult = await _userManager.CreateAsync(user, "PortalUser@123");
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        return BadRequest(ApiResponse<string>.Fail($"فشل إنشاء حساب مستخدم: {errors}"));
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                }
            }

            // 3. Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.GenerateToken(user, roles);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                Roles = roles,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            return Ok(ApiResponse<AuthResponseDto>.Ok(response));
        }

        [HttpPost("sync-documents")]
        [Authorize]
        public async Task<IActionResult> SyncDocuments([FromBody] SyncDocsRequestDto dto)
        {
            var userId = GetUserId();
            var citizenProfile = await _context.CitizenProfiles.FirstOrDefaultAsync(c => c.UserId == userId);

            string nationalId = dto?.NationalId ?? citizenProfile?.NationalId;
            if (string.IsNullOrWhiteSpace(nationalId))
            {
                return BadRequest(ApiResponse<string>.Fail("يرجى توفير الرقم القومي لإتمام سحب المستندات"));
            }

            // If user has profile but no National ID linked, update it
            if (citizenProfile != null)
            {
                if (string.IsNullOrWhiteSpace(citizenProfile.NationalId))
                {
                    citizenProfile.NationalId = nationalId;
                    citizenProfile.IsVerifiedViaDigitalPortal = true;
                    _context.Update(citizenProfile);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Create profile if none exists
                var newProfile = new CitizenProfile
                {
                    UserId = userId,
                    NationalId = nationalId,
                    IsVerifiedViaDigitalPortal = true,
                    FullName = User.Identity?.Name ?? "مواطن رقمي",
                    DateOfBirth = DateTime.UtcNow.AddYears(-25), // Mock age
                    CreatedAt = DateTime.UtcNow
                };
                _context.CitizenProfiles.Add(newProfile);
                await _context.SaveChangesAsync();
            }

            var syncResult = await _portalService.SyncCitizenDocumentsAsync(userId, nationalId);
            if (!syncResult.Success)
            {
                return BadRequest(ApiResponse<string>.Fail(syncResult.Message));
            }

            return Ok(ApiResponse<SyncDocumentsResultDto>.Ok(syncResult));
        }
    }

    public class SyncDocsRequestDto
    {
        public string? NationalId { get; set; }
    }
}
