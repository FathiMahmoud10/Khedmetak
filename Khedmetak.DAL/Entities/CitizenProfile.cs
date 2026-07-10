using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khedmetak.DAL.Entities
{
    using System;

    namespace Khedmetak.DAL.Entities
    {
        public class CitizenProfile
        {
            public int Id { get; set; }
            public string FullName { get; set; } = string.Empty;
            public DateTime DateOfBirth { get; set; }
            public string NationalId { get; set; } = string.Empty;
            public bool IsVerifiedViaDigitalPortal { get; set; } = false;
            public string City { get; set; } = string.Empty;
            public string District { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            public string BuildingNumber { get; set; } = string.Empty;
            public string FloorNumber { get; set; } = string.Empty;
            public string ApartmentNumber { get; set; } = string.Empty;
            public string PostalCode { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public int UserId { get; set; }
            public User User { get; set; } = null!;
        }
    }
}
