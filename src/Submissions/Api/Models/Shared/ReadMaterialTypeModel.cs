using System;
namespace Biobanks.Submissions.Api.Models.Shared
{
    public class ReadMaterialTypeModel : MaterialTypeModel
    {
        //Sum of all Collections and Capabilities
        public int MaterialDetailCount { get; set; }

        public bool UsedByExtractionProcedures { get; set; }
    }
}

