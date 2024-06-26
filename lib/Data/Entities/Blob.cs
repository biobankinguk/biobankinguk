using System.ComponentModel.DataAnnotations;

namespace Biobanks.Data.Entities
{
    public class Blob
    {
        public int Id { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        public byte[] Content { get; set; }

        public string ContentDisposition { get; set; }
    }
}
