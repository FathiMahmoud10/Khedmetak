using System.ComponentModel.DataAnnotations;

namespace Khedmetak.BLL.DTOS.Admin
{
    public class ServiceFeeTierAdminDto
    {
        public int Id { get; set; }
        public string TierName { get; set; } = string.Empty;
        public decimal Fees { get; set; }
        public string Duration { get; set; } = string.Empty;
        public bool IsRefundable { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class CreateServiceFeeTierDto
    {
        [Required, MaxLength(100)]
        public string TierName { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Fees { get; set; }

        [MaxLength(100)]
        public string Duration { get; set; } = string.Empty;

        public bool IsRefundable { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateServiceFeeTierDto
    {
        [Required, MaxLength(100)]
        public string TierName { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Fees { get; set; }

        [MaxLength(100)]
        public string Duration { get; set; } = string.Empty;

        public bool IsRefundable { get; set; }
        public int DisplayOrder { get; set; }
    }
}
