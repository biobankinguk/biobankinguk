namespace Biobanks.Web.SchemaModels
{
    // MIABIS 2.0 Core MIABIS-2.0-07 ('Contact information)
    public class PostalAddressSchemaModel
    {
        public string Type { get; set; } = "PostalAddress";
        public string ContactType { get; set; } // for sample resource will be 'Juristic Person' (MIABIS-2.0-05)
        public string Telephone { get; set; } // for Phone (MIABIS-2.0-07-C)
        public string StreetAddress { get; set; } // for Address (MIABIS-2.0-07-E)
        public string PostalCode { get; set; } // for ZIP (MIABIS-2.0-07-F)
        public string AddressLocality { get; set; } // for City (MIABIS-2.0-07-G)
        public string AddressCountry { get; set; } // for Country (MIABIS-2.0-07-H)
    }
}