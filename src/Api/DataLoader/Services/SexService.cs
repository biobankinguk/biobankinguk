using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Biobanks.DataLoader.Services
{
    public class SexService : IDataLoadService
    {
        private int _totalRecords;
        public async Task LoadData()
        {
            await SubmitData(PrepData().ToList());
        }

        private async Task SubmitData(IList<string> data)
        {
            //We post ontologies individually.

            var timer = Stopwatch.StartNew();
            for (var i = 0; i < data.Count; i++)
            {
                var sex = data[i];

                var result = await Program.SendJsonAsync("sex", JsonConvert.SerializeObject(sex));

                Console.WriteLine(
                    $"Sex {sex}" +
                    $" ({i} / {_totalRecords}):" +
                    $" Post {(result ? "successful" : "failed")}");
            }

            timer.Stop();
            Console.WriteLine($"Sex Posts took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");
        }

        private IEnumerable<string> PrepData()
        {
            var timer = Stopwatch.StartNew();

            //abuse config builders to load the data ;)
            var data = new ConfigurationBuilder()
                .AddJsonFile("StaticData/Sex.json", optional: false)
                .Build();

            var values = data.GetSection("Sex")
                .GetChildren()
                .AsEnumerable()
                .Select(x => x.Value)
                .ToList();

            _totalRecords = values.Count;

            var result = values.ToList();

            timer.Stop();
            Console.WriteLine($"Sex data prepared: {_totalRecords} record(s).");
            Console.WriteLine($"Preparation took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");

            return result;
        }
    }
}
