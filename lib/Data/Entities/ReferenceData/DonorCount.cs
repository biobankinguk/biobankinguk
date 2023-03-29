namespace Biobanks.Data.Entities.ReferenceData
{
    public class DonorCount : BaseReferenceData
    {
        public int? LowerBound { get; set; }

        public int? UpperBound { get; set; }
    }
}
