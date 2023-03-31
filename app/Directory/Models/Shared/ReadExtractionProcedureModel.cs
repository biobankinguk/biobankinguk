using System.Collections.Generic;

namespace Biobanks.Directory.Models.Shared
{
    public class ReadExtractionProcedureModel : OntologyTermModel
    {
        //Sum of all Material details (belonging to sample sets) with extraction procedure
        public int MaterialDetailsCount { get; set; }
        public List<int> MaterialTypeIds { get; set; }
    }
}

