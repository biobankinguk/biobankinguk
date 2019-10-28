using System;

namespace Common.Data.Identity
{
    public class TokenRecord
    {
        public TokenRecord(string purpose, string token, string userId, string eventType)
        {
            Purpose = purpose;
            Token = token;
            UserId = userId;
            Event = eventType;
        }

        public int Id { get; set; }

        public string Token { get; set; }
        public string Purpose { get; set; }
        public string UserId { get; set; }

        public string Event { get; set; }
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}
