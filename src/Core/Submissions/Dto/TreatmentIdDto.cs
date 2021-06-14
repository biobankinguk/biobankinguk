using System;
using System.ComponentModel.DataAnnotations;
using Biobanks.Entities.Api.Contracts;

namespace Core.Submissions.Dto
{
    public class TreatmentIdDto : ISubmissionTimestamped, IOrganisationOwnedDto
    {
        public DateTimeOffset SubmissionTimestamp { get; set; }

        public int OrganisationId { get; set; }

        [Required]
        public string IndividualReferenceId { get; set; }

        public DateTime DateTreated { get; set; }

        [Required]
        public string TreatmentCode { get; set; }
    }
}
