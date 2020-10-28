using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.Common.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Biobanks.DataLoader.Services
{
    public class MaterialTypeService : IDataLoadService
    {
        private int _totalRecords;

        public async Task LoadData()
        {
            await SubmitData(await PrepData());
        }

        private async Task SubmitData(ICollection<MaterialTypeJsonModel> data)
        {
            //work with batches
            var offset = 0;
            while (offset < data.Count)
            {
                var batch = data.Skip(offset).Take(Program.BatchSize).ToList();

                var timer = Stopwatch.StartNew();

                var batchResult = await Program.SendJsonAsync("materialtype/batch", JsonConvert.SerializeObject(batch));

                timer.Stop();
                Console.WriteLine(
                    $"Batch {offset} - {offset + batch.Count}" +
                    $" / {_totalRecords}:" +
                    $" Post {(batchResult ? "successful" : "failed")}");
                Console.WriteLine($"Post took: {timer.Elapsed}");
                Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");

                offset += batch.Count;
            }
        }

        private async Task<ICollection<MaterialTypeJsonModel>> PrepData()
        {
            //get all the groups
            var timer = Stopwatch.StartNew();

            var groups = JsonConvert.DeserializeObject<IEnumerable<MaterialTypeGroup>>(
                await Program.GetJsonAsync("materialtypegroup")).ToList();

            timer.Stop();
            Console.WriteLine($"Material Type Groups fetched: {groups.Count} in {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");

            //abuse config builders to load the data ;)
            timer = Stopwatch.StartNew();
            var data = new ConfigurationBuilder()
                .AddJsonFile("StaticData/MaterialType.json", optional: false)
                .Build();

            var values = data.GetSection("MaterialType")
                .GetChildren()
                .Select(x => new MaterialTypeJsonModel
                {
                    Value = x["Value"],
                    MaterialTypeGroupNames = x.GetSection("MaterialTypeGroupNames")
                        .GetChildren()
                        .Select(y => y.Value)
                        .ToList()
                })
                .ToList();

            _totalRecords = values.Count;

            timer.Stop();
            Console.WriteLine($"MaterialType data prepared: {_totalRecords} records.");
            Console.WriteLine($"Preparation took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");

            return values;
        }
    }
}
