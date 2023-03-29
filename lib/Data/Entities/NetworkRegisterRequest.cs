using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Data.Entities
{
    public class NetworkRegisterRequest
    {
        public int NetworkRegisterRequestId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserEmail { get; set; }

        [Required]
        public string NetworkName { get; set; }


        public DateTime RequestDate { get; set; }

        public DateTime? AcceptedDate { get; set; }

        public DateTime? NetworkCreatedDate { get; set; }

        public DateTime? DeclinedDate { get; set; }
    }
}
