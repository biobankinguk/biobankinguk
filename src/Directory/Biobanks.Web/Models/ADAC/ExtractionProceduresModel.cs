using Biobanks.Entities.Shared.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class ExtractionProceduresModel
    {
        public ICollection<ReadExtractionProcedureModel> ExtractionProcedures { get; set; }

        public IEnumerable<MaterialType> MaterialTypes { get; set; }
    }
}