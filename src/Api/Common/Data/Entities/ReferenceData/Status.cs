using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Common.Data.Entities.ReferenceData
{
    public class Status
    {
        //We want Identity Insert for Reference Data
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Value { get; set; }
    }
}
