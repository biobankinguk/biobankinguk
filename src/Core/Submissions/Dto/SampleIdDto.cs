using System;
using System.ComponentModel.DataAnnotations;
using Biobanks.Entities.Api.Contracts;

namespace Core.Submissions.Dto
{
    public class SampleIdDto : ISubmissionTimestamped, IOrganisationOwnedDto
    {
        public DateTimeOffset SubmissionTimestamp { get; set; }

        public bool Live { get; set; }

        public int OrganisationId { get; set; }

        [Required]
        public string IndividualReferenceId { get; set; }

        [Required]
        public string Barcode { get; set; }

        public string CollectionName { get; set; }
    }
}
