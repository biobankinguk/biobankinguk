using System.Collections.Generic;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.Biobank
{
    public class BiobankAdminsModel
    {
        public int BiobankId { get; set; }

        public ICollection<RegisterEntityAdminModel> Admins { get; set; }
    }
}