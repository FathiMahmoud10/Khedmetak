using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.BLL.DTOS.Profile
{
    public class UserProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? NationalId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Street { get; set; }
        public string? BuildingNumber { get; set; }
        public string? FloorNumber { get; set; }
        public string? ApartmentNumber { get; set; }
        public string? PostalCode { get; set; }
        public string AvatarUrl { get; set; } = string.Empty;
    }

    public class UpdateUserProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? NationalId { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Street { get; set; }

        // اختياريين - بيتبعتوا بس لو المستخدم فعلاً بيغيّر الباسورد
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}

