using System;
using System.ComponentModel.DataAnnotations;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{
    public class SubmitServiceRequestDto
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string UserEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "معرف الخدمة مطلوب")]
        public int GovServiceId { get; set; }
    }
}
