using System.Collections.Generic;

namespace Biobanks.Directory.Models.Shared
{
    public class CountiesModel
    {
        public Dictionary<string, IEnumerable<CountyModel>> Counties { get; set; }
    }

    public class CountyModel
    {
        public int Id { get; set; }

        public int? CountryId { get; set; }

        public string Name { get; set; }

        public int CountyUsageCount { get; set; }
    }
}

