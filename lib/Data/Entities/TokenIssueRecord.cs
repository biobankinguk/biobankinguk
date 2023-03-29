using System;

namespace Biobanks.Data.Entities
{
    public class TokenIssueRecord
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public string Purpose { get; set; }

        public string UserId { get; set; }

        public DateTime IssueDate { get; set; }
    }
}
