using System;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{
    public class ServiceRequestResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public Guid SessionGuid { get; set; }
    }
}
