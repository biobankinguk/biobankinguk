using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Data.Entities
{
    public class RegistrationDomainRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RuleType { get; set; }

        [Required]
        public string Value { get; set; }

        public string Source { get; set; }

        public DateTime? DateModified { get; set; }
    }
}
