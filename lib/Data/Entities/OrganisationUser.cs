using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Data.Entities
{
    public class OrganisationUser //M <-> M map for Organisations and Organisation Users
    {
        [Key, Column(Order = 0)]
        public int OrganisationId { get; set; }
        public Organisation Organisation { get; set; } //this doesn't seem to populate?

        [Key, Column(Order = 1)]
        public string OrganisationUserId { get; set; } //Here's where we leave it in th Biobanks DB side, Identity can pick it up here by id
    }
}
