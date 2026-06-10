using System.ComponentModel.DataAnnotations;

namespace Khedmetak.BLL.DTOS.Admin
{
    /// <summary>
    /// DTO used by the Admin to CREATE a new government service.
    /// </summary>
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
    }

    /// <summary>
    /// DTO used by the Admin to UPDATE an existing government service.
    /// </summary>
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
    }
}
