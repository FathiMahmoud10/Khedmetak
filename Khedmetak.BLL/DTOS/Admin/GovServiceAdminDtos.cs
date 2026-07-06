using System.ComponentModel.DataAnnotations;

namespace Khedmetak.BLL.DTOS.Admin
{
   
    public class CreateGovServiceDto
    {
        [Required, MaxLength(200)]
        public string SrvName { get; set; } = string.Empty;

        [Required]
        public string SrvDesc { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal SrvFees { get; set; }

        [Required, MaxLength(100)]
        public string SrvTime { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal EstimatedFees { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [MaxLength(200)]
        public string ProviderEntity { get; set; } = string.Empty;

        [MaxLength(200)]
        public string TargetAudience { get; set; } = string.Empty;

        [MaxLength(200)]
        public string DeliveryMethod { get; set; } = string.Empty;

        public bool NeedsGuarantee { get; set; }
    }

 
    public class UpdateGovServiceDto
    {
        [Required, MaxLength(200)]
        public string SrvName { get; set; } = string.Empty;

        [Required]
        public string SrvDesc { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal SrvFees { get; set; }

        [Required, MaxLength(100)]
        public string SrvTime { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal EstimatedFees { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [MaxLength(200)]
        public string ProviderEntity { get; set; } = string.Empty;

        [MaxLength(200)]
        public string TargetAudience { get; set; } = string.Empty;

        [MaxLength(200)]
        public string DeliveryMethod { get; set; } = string.Empty;

        public bool NeedsGuarantee { get; set; }
    }

    public class UpdateFeesDto
    {
        public decimal SrvFees { get; set; }
        public decimal EstimatedFees { get; set; }
    }
}
