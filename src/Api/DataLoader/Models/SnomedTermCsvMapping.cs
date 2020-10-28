using TinyCsvParser.Mapping;

namespace Biobanks.DataLoader.Models
{
    public class SnomedTermCsvMapping : CsvMapping<SnomedTermCsvModel>
    {
        public SnomedTermCsvMapping(bool hasTags = true) : base()
        {
            MapProperty(0, x => x.Id);
            MapProperty(1, x => x.Description);
            //Ignore column 2
            if (hasTags) MapProperty(3, x => x.SnomedTag);
        }
    }
}
