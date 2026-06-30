// Khedmetak.BLL/DTOS/Auth/RegisterDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace Khedmetak.BLL.DTOS.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "الإيميل مطلوب")]
        [EmailAddress(ErrorMessage = "إيميل غير صحيح")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "الباسورد مطلوب")]
        [MinLength(6, ErrorMessage = "الباسورد لازم يكون 6 حروف على الأقل")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "تأكيد الباسورد مطلوب")]
        [Compare("Password", ErrorMessage = "الباسورد مش متطابق")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // CitizenProfile fields
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "المنطقة مطلوبة")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "الشارع مطلوب")]
        public string Street { get; set; } = string.Empty;

        public string BuildingNumber { get; set; } = string.Empty;
        public string FloorNumber { get; set; } = string.Empty;
        public string ApartmentNumber { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرقم القومي مطلوب")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "الرقم القومي يجب أن يكون 14 رقماً")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public string Phone { get; set; } = string.Empty;
    }
}