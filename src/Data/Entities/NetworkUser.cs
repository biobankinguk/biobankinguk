using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Entities.Data
{
    public class NetworkUser
    {
        [Key, Column(Order = 0)]
        public int NetworkId { get; set; }
        public Network Network { get; set; } //this doesn't seem to populate?

        [Key, Column(Order = 1)]
        public string NetworkUserId { get; set; } //Here's where we leave it in the Biobanks DB side, Identity can pick it up here by id
    }
}
