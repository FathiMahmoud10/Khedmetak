// Khedmetak.API/Controllers/AuthController.cs
using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Auth;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Entities.Khedmetak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Khedmetak.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtService _jwtService;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            //  ابحث عن اليوزر
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized(ApiResponse<string>.Fail("الإيميل أو الباسورد غلط - المستخدم غير موجود في قاعدة البيانات"));

            //  تحقق من الباسورد
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                string reason = "الإيميل أو الباسورد غلط";
                if (result.IsLockedOut)
                    reason += " (الحساب مغلق)";
                if (result.IsNotAllowed)
                    reason += " (الدخول غير مسموح - تأكد من تفعيل الحساب)";

                return Unauthorized(ApiResponse<string>.Fail(reason));
            }

            //  جيب الـ roles
            var roles = await _userManager.GetRolesAsync(user);

            //  اعمل التوكن
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

        // Khedmetak.API/Controllers/AuthController.cs - أضف الـ endpoint ده جوه الـ class
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // تحقق إن الإيميل مش موجود
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(ApiResponse<string>.Fail("الإيميل ده مسجل بالفعل"));

            // عمل اليوزر
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Role = "User",                 // ⬅️ كان فاضي وعمود [Required]، ده سبب فشل CreateAsync بصمت
                EmailConfirmed = true,          // ⬅️ كان false، يمنع تسجيل الدخول لو فيه RequireConfirmedAccount
                CreatedAt = DateTime.UtcNow,
                CitizenProfile = new CitizenProfile
                {
                    FullName = dto.FullName,
                    DateOfBirth = dto.DateOfBirth,
                    City = dto.City,
                    District = dto.District,
                    Street = dto.Street,
                    BuildingNumber = dto.BuildingNumber,
                    FloorNumber = dto.FloorNumber,
                    ApartmentNumber = dto.ApartmentNumber,
                    PostalCode = dto.PostalCode,
                }
            };

            // CreateAsync هي اللي بتحط PasswordHash تلقائيًا، فلازم نتأكد من نتيجتها
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<string>.Fail(errors));
            }

            // ⬅️ لازم نتأكد إن إضافة الـ Role نجحت برضه
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<string>.Fail("فشل ربط المستخدم بالـ Role: " + errors));
            }

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
    }
}