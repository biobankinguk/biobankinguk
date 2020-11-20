using System.Collections.Generic;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class NetworkModel
    {
        public int NetworkId { get; set; }

        public string Name { get; set; }

        public ICollection<RegisterEntityAdminModel> Admins { get; set; }
    }
}