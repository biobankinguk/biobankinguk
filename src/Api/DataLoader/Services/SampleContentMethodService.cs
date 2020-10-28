using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Biobanks.DataLoader.Services
{
    public class SampleContentMethodService : IDataLoadService
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
                var sampleContentMethod = data[i];

                var result = await Program.SendJsonAsync("SampleContentMethod", JsonConvert.SerializeObject(sampleContentMethod));

                Console.WriteLine(
                    $"SampleContentMethod {sampleContentMethod}" +
                    $" ({i} / {_totalRecords}):" +
                    $" Post {(result ? "successful" : "failed")}");
            }

            timer.Stop();
            Console.WriteLine($"SampleContentMethod Posts took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");
        }

        private IEnumerable<string> PrepData()
        {
            var timer = Stopwatch.StartNew();

            //abuse config builders to load the data ;)
            var data = new ConfigurationBuilder()
                .AddJsonFile("StaticData/SampleContentMethod.json", optional: false)
                .Build();

            var values = data.GetSection("SampleContentMethod")
                .GetChildren()
                .AsEnumerable()
                .Select(x => x.Value)
                .ToList();

            _totalRecords = values.Count;

            var result = values.ToList();

            timer.Stop();
            Console.WriteLine($"SampleContentMethod data prepared: {_totalRecords} record(s).");
            Console.WriteLine($"Preparation took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");

            return result;
        }
    }
}
