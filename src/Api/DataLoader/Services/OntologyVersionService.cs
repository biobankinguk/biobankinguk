using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Common.Data.Entities.ReferenceData;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Biobanks.DataLoader.Services
{
    public class OntologyVersionService : IDataLoadService
    {
        private int _totalRecords;
        public async Task LoadData()
        {
            await SubmitData(PrepData().ToList());
        }

        private async Task SubmitData(IList<OntologyVersion> data)
        {
            //We post ontologies individually.

            var timer = Stopwatch.StartNew();
            for (var i = 0; i < data.Count; i++)
            {
                var ontologyVersion = data[i];

                var result = await Program.SendJsonAsync("ontologyversion", JsonConvert.SerializeObject(ontologyVersion));

                Console.WriteLine(
                    $"OntologyVersion {ontologyVersion}" +
                    $" ({i} / {_totalRecords}):" +
                    $" Post {(result ? "successful" : "failed")}");
            }

            timer.Stop();
            Console.WriteLine($"Ontology Posts took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");
        }

        private IEnumerable<OntologyVersion> PrepData()
        {
            var timer = Stopwatch.StartNew();

            //abuse config builders to load the data ;)
            var data = new ConfigurationBuilder()
                .AddJsonFile("StaticData/OntologyVersion.json", optional: false)
                .Build();

            var values = data.GetSection("OntologyVersion")
                .GetChildren()
                .AsEnumerable()
                .Select(x => new OntologyVersion
                {
                    Value = x["Value"],
                    Ontology = new Ontology
                    {
                        Value = x.GetSection("Ontology")["Value"]
                    }
                })
                .ToList();

            _totalRecords = values.Count;

            var result = values.ToList();

            timer.Stop();
            Console.WriteLine($"Ontology data prepared: {_totalRecords} record(s).");
            Console.WriteLine($"Preparation took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");

            return result;
        }
    }
}
