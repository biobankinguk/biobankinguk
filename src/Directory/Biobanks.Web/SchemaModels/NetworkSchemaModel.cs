using System.Collections.Generic;

namespace Biobanks.Web.SchemaModels
{
    /*
     TODO When deserializing to e.g. JSON-LD use ContractResolver = new CamelCasePropertyNamesContractResolver()
     to conform with schema.org standards
    */

    // No corresponding MIABIS Core 2.0 type
    public class NetworkSchemaModel
    {
        public string SchemaDataContext { get; set; } = "http://schema.org";
        public string SchemaDataType { get; set; } = "Organization";
        public string Url { get; set; }
        public string Identifier { get; set; } 
        public string Name { get; set; }
        public string Email { get; set; }
        public string SameAs { get; set; } // Url to a page uniquely describing the org
        public string Description { get; set; }
        public string AlternativeName { get; set; } // for Acronym
        public string LegalName { get { return Name; } }

        // TODO implement organization topics
        public IEnumerable<string> Topic { get; set; } // EDAM:Topic types describing scientified topic

        // TODO implement organization types - probably need a drop-down in Network Edit view, in addition to them all being consortiums
        public IEnumerable<string> Type { get; set; } = new List<string> { "consortium" };
        /* 
            current permitted values:
            single-company corporation,
            multinational,
            not-for-profit,
            alliance,
            consortium,
            institute,
            department,
            working party,
            project. 
        */

        // TODO best way of deserializing this and also preventing circular references?
        public IEnumerable<SampleResourceSchemaModel> Member { get; set; } = new List<SampleResourceSchemaModel>();
    }
}