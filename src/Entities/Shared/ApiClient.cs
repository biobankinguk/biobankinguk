namespace Biobanks.Entities.Shared
{
    public class ApiClient
    {
        public int Id { get; set; }

        public string ClientId { get; set; }

        public string ClientSecretHash { get; set; }
    }
}
