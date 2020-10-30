using System.Collections.Generic;

namespace Biobanks.Web.Models.ADAC
{
    public class RequestsModel
    {
        public ICollection<BiobankRequestModel> BiobankRequests { get; set; }
        public ICollection<NetworkRequestModel> NetworkRequests { get; set; }
    }
}