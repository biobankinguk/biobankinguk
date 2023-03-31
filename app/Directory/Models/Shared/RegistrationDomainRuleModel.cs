using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class RegistrationDomainRuleModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string RuleType { get; set; }

        [Required]
        public string Value { get; set; }

        public string Source { get; set; }

        public DateTime? DateModified { get; set; }
        
    }
}
