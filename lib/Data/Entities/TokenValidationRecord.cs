using System;

namespace Biobanks.Data.Entities
{
    public class TokenValidationRecord
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public string Purpose { get; set; }

        public string UserId { get; set; }

        public DateTime ValidationDate { get; set; }

        public bool ValidationSuccessful { get; set; }
    }
}
