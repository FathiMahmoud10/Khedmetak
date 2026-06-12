// Khedmetak.API/Controllers/AuthController.cs
using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Auth;
using Khedmetak.BLL.Services.Implementation;
using Khedmetak.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

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
    }
}