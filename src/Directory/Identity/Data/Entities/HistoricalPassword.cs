using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Identity.Data.Entities
{
    public class HistoricalPassword
    {
        public HistoricalPassword()
        {
            CreatedDate = DateTimeOffset.Now;
        }

        [Key, Column(Order = 0)]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Key, Column(Order = 1)]
        public string PasswordHash { get; set; }


        public DateTimeOffset CreatedDate { get; set; }
    }
}
