﻿using Nest;

namespace Biobanks.Directory.Search.Dto.Documents
{
    public class BaseDocument
    {
        [Keyword(Name = "ontologyTerm")]
        public string OntologyTerm { get; set; }

        [Keyword(Name = "biobankExternalId")]
        public string BiobankExternalId { get; set; }
    }
}
