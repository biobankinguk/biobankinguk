namespace Biobanks.Web.Utilities
{
    public enum ActiveOrganisationType {None, Biobank, Network, NewBiobank, NewNetwork}

    public static class SessionKeys
    {
        public static string ActiveOrganisationId => "ActiveOrganisationId";
        public static string ActiveOrganisationName => "ActiveOrganisationName";
        public static string ActiveOrganisationType => "ActiveOrgansiationType";
    }
}