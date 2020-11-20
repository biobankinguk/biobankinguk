using System.Collections.Generic;

namespace Biobanks.Web.Models.ADAC
{
    public class HistoricalModel
    {
        public ICollection<HistoricalRequestModel> HistoricalRequests { get; set; }
    }
}