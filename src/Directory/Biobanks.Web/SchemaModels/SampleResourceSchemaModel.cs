using System.Collections.Generic;

namespace Biobanks.Web.SchemaModels
{
    /*
     TODO When deserializing to e.g. JSON-LD use ContractResolver = new CamelCasePropertyNamesContractResolver()
     to conform with schema.org standards
    */

    // MIABIS 2.0 Core ('Data describing Biobank')
    public class SampleResourceSchemaModel
    {
        public string SchemaDataContext { get; set; } = "http://schema.org";
        public string SchemaDataType { get; set; } = "Organization";
        public string Url { get; set; }
        public string Identifier { get; set; } // for ID (MIABIS-2.0-01)
        public string Name { get; set; }
        public string Email { get; set; } // for Contact Information -> Email (MIABIS-2.0-07-D)
        public string SameAs { get; set; } // Url to a page uniquely describing the org
        public PostalAddressSchemaModel Address { get; set; }
        public string Description { get; set; }
        public string AlternativeName { get; set; } // for Acronym (MIABIS 2.0-02)
        public string LegalName { get; set; } // for Juristic Person (MIABIS 2.0-05)

        // TODO implement organization topics
        // EDAM:Topic types describing scientified topic
        public IEnumerable<string> Topic { get; set; } = new List<string>(); 

        // TODO implement organization types - presumably mostly not-for-profit?
        public IEnumerable<string> Type { get; set; } = new List<string>();
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
        public IEnumerable<NetworkSchemaModel> MemberOf { get; set; } = new List<NetworkSchemaModel>();
    }
}