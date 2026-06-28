using System;

namespace Khedmetak.AI.DTOs.ChatSessionDTO
{
    /// <summary>
    /// A lightweight summary of a user's chat session – used for the chat history list.
    /// </summary>
    public class UserSessionSummaryDTO
    {
        public int    Id            { get; set; }
        public Guid   SessionGuidId { get; set; }
        public DateTime StartedAt   { get; set; }
        public DateTime? EndedAt    { get; set; }

        /// <summary>First user message text (preview), empty when session just started.</summary>
        public string  Preview      { get; set; } = string.Empty;

        /// <summary>Total number of user messages in this session.</summary>
        public int     MessageCount { get; set; }
    }
}
