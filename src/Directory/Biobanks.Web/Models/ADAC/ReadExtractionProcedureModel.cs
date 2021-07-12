using System.Collections.Generic;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadExtractionProcedureModel : OntologyTermModel
    {
        //Sum of all Material details (belonging to sample sets) with extraction procedure
        public int MaterialDetailsCount { get; set; }
        public List<int> MaterialTypeIds { get; set; }
    }
}