using Khedmetak.BLL.ApiResponse;
using Khedmetak.BLL.DTOS.Profile;
using Khedmetak.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Khedmetak.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public ProfileController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET api/Profile
        // بيرجع نفس البيانات اللي المستخدم سجّل بيها بالظبط (مش بيانات من الـ JWT فقط)
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponse<string>.Fail("غير مصرح لك، يرجى تسجيل الدخول أولاً"));

            // Include بتجيب الـ CitizenProfile المرتبط باليوزر (نفس اللي اتحفظ وقت التسجيل)
            var user = await _userManager.Users
                .Include(u => u.CitizenProfile)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null)
                return NotFound(ApiResponse<string>.Fail("المستخدم غير موجود"));

            var dto = new UserProfileDto
            {
                FullName = user.CitizenProfile?.FullName ?? user.Name ?? user.Email!,
                Email = user.Email!,
                Phone = user.PhoneNumber,
                NationalId = user.CitizenProfile?.NationalId,
                DateOfBirth = user.CitizenProfile?.DateOfBirth,
                City = user.CitizenProfile?.City,
                District = user.CitizenProfile?.District,
                Street = user.CitizenProfile?.Street,
                BuildingNumber = user.CitizenProfile?.BuildingNumber,
                FloorNumber = user.CitizenProfile?.FloorNumber,
                ApartmentNumber = user.CitizenProfile?.ApartmentNumber,
                PostalCode = user.CitizenProfile?.PostalCode,
                AvatarUrl = $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(user.CitizenProfile?.FullName ?? user.Name ?? "User")}&background=298b64&color=fff"
            };

            return Ok(ApiResponse<UserProfileDto>.Ok(dto));
        }

        // PUT api/Profile
        // بيحدّث نفس الـ CitizenProfile اللي اتعمل وقت الـ Register، وبيغيّر الباسورد لو المستخدم طلب كده
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(ApiResponse<string>.Fail("غير مصرح لك، يرجى تسجيل الدخول أولاً"));

            var user = await _userManager.Users
                .Include(u => u.CitizenProfile)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null)
                return NotFound(ApiResponse<string>.Fail("المستخدم غير موجود"));

            user.Name = dto.FullName;
            user.PhoneNumber = dto.Phone;

            if (user.CitizenProfile != null)
            {
                user.CitizenProfile.FullName = dto.FullName;
                user.CitizenProfile.NationalId = dto.NationalId ?? user.CitizenProfile.NationalId;
                user.CitizenProfile.City = dto.City;
                user.CitizenProfile.District = dto.District;
                user.CitizenProfile.Street = dto.Street;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<string>.Fail(errors));
            }

            // تغيير الباسورد اختياري - بس لو المستخدم بعت القيمتين
            if (!string.IsNullOrWhiteSpace(dto.CurrentPassword) && !string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                var pwResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!pwResult.Succeeded)
                {
                    var errors = string.Join(", ", pwResult.Errors.Select(e => e.Description));
                    return BadRequest(ApiResponse<string>.Fail("فشل تغيير كلمة المرور: " + errors));
                }
            }

            return Ok(ApiResponse<string>.Ok("تم التحديث", "تم حفظ بياناتك بنجاح"));
        }

        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idClaim, out var id) ? id : null;
        }
    }
}
