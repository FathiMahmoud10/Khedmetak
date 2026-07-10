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
using Khedmetak.BLL.DTOS.DigitalPortal;

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

        /*
         هنا بعمل العمليات الاساسية اللي بتتعمل ف بوابة مصر الرقمية
        زي ارسال OTP والتحقق منه، 
        وبعدين بعمل تسجيل دخول تلقائي للمستخدم لو الرقم القومي متحقق منه،
        ولو مش موجود ف قاعدة البيانات بعمل تسجيل دخول تلقائي مع انشاء حساب
        جديد للمستخدم وربطه بالرقم القومي المتحقق منه.
         
         
         By Engineer: Fathi Mahmoud 
         */

        // Helper method to get the current user's ID from the JWT claims
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

            return Ok(ApiResponse<string>.Ok("تم إرسال كود التحقق (123456) إلى هاتفك بنجاح"));
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


            //   بتأكد ان المواطن مسجل ف قاعدة البيانات بتاعتنا ولا لأ
            var profile = await _context.CitizenProfiles
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.NationalId == citizen.NationalId);

            User user;
            if (profile != null)
            {
                // 1. User exists, proceed to generate JWT
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

            // لو عنده رقم قومي محفوظ ومتحقق منه بالفعل، بنستخدمه دايمًا
            // وبنتجاهل أي رقم قومي جاي من الـ request عشان نمنع الـ override / تبديل الهوية
            string nationalId;

            if (citizenProfile != null && !string.IsNullOrWhiteSpace(citizenProfile.NationalId) && citizenProfile.IsVerifiedViaDigitalPortal)
            {
                nationalId = citizenProfile.NationalId;
            }
            else
            {
                // مفيش رقم قومي متحقق منه محفوظ لسه -> لازم تحقق OTP قبل الربط
                if (string.IsNullOrWhiteSpace(dto?.NationalId) )
                {
                    return BadRequest(ApiResponse<string>.Fail("يجب التحقق من الرقم القومي عن طريق كود التحقق قبل سحب المستندات"));
                }

                DigitalPortalCitizenDto verifiedCitizen;
                try
                {
                    verifiedCitizen = await _portalService.VerifyOtpAndGetCitizenAsync(new DigitalPortalOtpDto
                    {
                        NationalId = dto.NationalId,
                    });
                }
                catch (Exception)
                {
                    return BadRequest(ApiResponse<string>.Fail("تعذر التحقق من كود التحقق حاليًا، برجاء المحاولة لاحقًا"));
                }

                if (verifiedCitizen == null)
                {
                    return BadRequest(ApiResponse<string>.Fail("كود التحقق غير صحيح أو غير متطابق"));
                }

                nationalId = verifiedCitizen.NationalId;

                if (citizenProfile != null)
                {
                    // بروفايل موجود بس من غير رقم قومي متحقق -> نكمّله ببيانات حقيقية من البوابة
                    citizenProfile.NationalId = nationalId;
                    citizenProfile.IsVerifiedViaDigitalPortal = true;
                    citizenProfile.FullName = verifiedCitizen.FullName ?? citizenProfile.FullName;
                    citizenProfile.DateOfBirth = verifiedCitizen.DateOfBirth;
                    _context.Update(citizenProfile);
                }
                else
                {
                    citizenProfile = new CitizenProfile
                    {
                        UserId = userId,
                        NationalId = nationalId,
                        IsVerifiedViaDigitalPortal = true,
                        FullName = verifiedCitizen.FullName ?? (User.Identity?.Name ?? "مواطن رقمي"),
                        DateOfBirth = verifiedCitizen.DateOfBirth, // بيانات حقيقية من البوابة،  
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.CitizenProfiles.Add(citizenProfile);
                }

                await _context.SaveChangesAsync();
            }

            SyncDocumentsResultDto syncResult;
            try
            {
                syncResult = await _portalService.SyncCitizenDocumentsAsync(userId, nationalId);
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<string>.Fail("حدث خطأ أثناء الاتصال بالبوابة الرقمية، برجاء المحاولة لاحقًا"));
            }

            if (!syncResult.Success)
            {
                return BadRequest(ApiResponse<string>.Fail(syncResult.Message));
            }

            return Ok(ApiResponse<SyncDocumentsResultDto>.Ok(syncResult));
        }
    }

 
}
