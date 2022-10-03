namespace Biobanks.Submissions.Api.Models.Directory
{
    public class CapabilityDTO
    {
        public int Id { get; set; }
        public int OrganisationId { get; set; }
        public string OntologyTerm { get; set; }
        public bool BespokeConsentForm { get; set; }
        public bool BespokeSOP { get; set; }
        public int? AnnualDonorExpectation { get; set; }

        public int SampleCollectionModeId
        {
            get
            {
                if (BespokeConsentForm && BespokeSOP)
                {
                    return 3;
                }

                if (!BespokeConsentForm && BespokeSOP)
                {
                    return 2;
                }

                if (BespokeConsentForm && !BespokeSOP)
                {
                    return 1;
                }

                return 4;
            }
        }
    }
}
