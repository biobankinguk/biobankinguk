using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Shared
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