using System.Collections.Generic;

namespace Biobanks.Web.Models.Network
{
    public class NetworkBiobankModel
    {
        public int BiobankId { get; set; }

        public string Name { get; set; }

        public ICollection<string> Admins { get; set; }
    }
}