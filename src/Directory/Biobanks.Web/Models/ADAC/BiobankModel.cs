using System.Collections.Generic;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class BiobankModel
    {
        public int BiobankId { get; set; }

        public string BiobankExternalId { get; set; }

        public string Name { get; set; }

        public bool IsSuspended { get; set; }

        public string ContactEmail {get; set; }

        public ICollection<RegisterEntityAdminModel> Admins { get; set; }
    }
}