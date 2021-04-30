﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Core.Models.OptionsModels
{

    public class MaterialTypesLegacyModel
    {
        [JsonProperty("listOfMappings")]
        public List<MaterialType> ListOfMappings { get; set; }
    }

    public class MaterialType
    {
        [JsonProperty("listOfMappings")]
        public OldMaterialType Old { get; set; }

        [JsonProperty("listOfMappings")]
        public NewMaterialType New { get; set; }
    }
    public class OldMaterialType
    {
        [JsonProperty("materialType")]
        public string MaterialType { get; set; }
    }

    public class NewMaterialType
    {
        [JsonProperty("materialType")]
        public string MaterialType { get; set; }

        [JsonProperty("extractionProcedure")]
        public string ExtractionProcedure { get; set; }

        [JsonProperty("extractionProcedureOntologyField")]
        public string ExtractionProcedureOntologyField { get; set; }
    }
}
