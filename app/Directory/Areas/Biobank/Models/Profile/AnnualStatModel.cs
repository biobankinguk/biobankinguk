namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;

public class AnnualStatModel
{
    public int AnnualStatisticId { get; set; }
    public int Year { get; set; }
    public int? Value { get; set; }
}
