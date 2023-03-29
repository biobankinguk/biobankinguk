using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Profile
{
    public class NetworkModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }

        public string Logo { get; set; }

        public string Description { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string ContactEmail { get; set; }

        [Display(Name = "SOP status")]
        public string SopStatus { get; set; }

        public ICollection<BiobankMemberModel> BiobankMembers { get; set; }
    }
}
