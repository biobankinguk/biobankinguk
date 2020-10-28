namespace Biobanks.DataLoader.Models
{
    // This class represents a SNOMED record from the CSV
    // (with a descriptive Tag string, rather than the id)
    public class SnomedTermCsvModel
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string SnomedTag { get; set; }
    }
}
