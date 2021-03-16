using Biobanks.Entities.Data;

using System.Collections.Generic;

namespace Biobanks.Entities.Shared
{
    public class ApiClient
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ClientId { get; set; }

        public string ClientSecretHash { get; set; }

        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
