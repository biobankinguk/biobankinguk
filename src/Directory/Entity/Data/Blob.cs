using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
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
