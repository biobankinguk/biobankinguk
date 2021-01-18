using Directory.Data.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class StorageTemperaturesModel
    {
        public ICollection<StorageTemperatureModel> StorageTemperatures { get; set; }
    }

    public class StorageTemperatureModel
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }

        public int SampleSetsCount { get; set; }
    }
}