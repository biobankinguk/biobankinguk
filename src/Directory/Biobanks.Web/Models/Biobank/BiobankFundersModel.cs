using System.Collections.Generic;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.Biobank
{
    public class BiobankFundersModel
    {
        public int BiobankId { get; set; }

        public ICollection<FunderModel> Funders { get; set; }
    }
}