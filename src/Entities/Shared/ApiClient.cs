using Biobanks.Entities.Data;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Shared
{
    public class ApiClient
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecretHash { get; set; }

        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
