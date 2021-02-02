using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data
{
    public class OrganisationRegisterRequest
    {
        public int OrganisationRegisterRequestId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserEmail { get; set; }

        public int OrganisationTypeId { get; set; }
        public OrganisationType OrganisationType { get; set; }

        [Required]
        public string OrganisationName { get; set; }


        public DateTime RequestDate { get; set; }

        public DateTime? AcceptedDate { get; set; }

        public DateTime? OrganisationCreatedDate { get; set; }

        public DateTime? DeclinedDate { get; set; }

        // the reslting organisation this is attached to
        public string OrganisationExternalId { get; set; }
    }
}
