using System.Collections.Generic;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.Network
{
    public class NetworkAdminsModel
    {
        public int NetworkId { get; set; }

        public ICollection<RegisterEntityAdminModel> Admins { get; set; }
    }
}