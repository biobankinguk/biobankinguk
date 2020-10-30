namespace Biobanks.Web.Models.Biobank
{
    public class AnnualStatModel
    {
        public int AnnualStatisticId { get; set; }
        public int Year { get; set; }
        public int? Value { get; set; }
    }
}