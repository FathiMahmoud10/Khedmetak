using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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

        // ── بيانات المستخدم ──
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Notes { get; set; }

        // ── الملفات (اختياري) ──
        public List<IFormFile>? Files { get; set; }
    }
}
