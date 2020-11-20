
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadAnnualStatisticGroupModel : AnnualStatisticGroupModel
    {
        //Count where AnnualStatisticGroup is referenced
        public int AnnualStatisticGroupCount { get; set; }
    }
}