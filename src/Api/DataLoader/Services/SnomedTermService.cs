using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.DataLoader.Models;
using Newtonsoft.Json;
using TinyCsvParser;
using TinyCsvParser.Tokenizer;

namespace Biobanks.DataLoader.Services
{
    public class SnomedTermService : IDataLoadService
    {
        private int _totalRecords;
        private int _invalidRecords;
        private int _duplicateRecords;
        private int _processedRecords;

        private CsvParser<SnomedTermCsvModel> _csvParser;

        public async Task LoadData()
        {
            // init the csv parser
            _csvParser = new CsvParser<SnomedTermCsvModel>(
                new CsvParserOptions(
                    skipHeader: false,
                    tokenizer: new QuotedStringTokenizer(',')),
                new SnomedTermCsvMapping(hasTags: true));

            await SubmitData(await PrepData());
        }

        private async Task SubmitData(ICollection<SnomedTerm> data)
        {
            // total records is now those processed for submission
            // not all CSV records
            _totalRecords = data.Count;

            //work with batches
            var offset = 0;
            while (offset < data.Count)
            {
                var batch = data.Skip(offset).Take(Program.BatchSize).ToList();

                var timer = Stopwatch.StartNew();

                var batchResult = await Program.SendJsonAsync("snomedterm/batch", JsonConvert.SerializeObject(batch));

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

        private async Task<ICollection<SnomedTerm>> PrepData()
        {
            // Get the valid list of tags from the data service
            var timer = Stopwatch.StartNew();

            var tags = JsonConvert.DeserializeObject<IEnumerable<SnomedTag>>(
                await Program.GetJsonAsync("snomedtag")).ToList();

            timer.Stop();
            Console.WriteLine($"SNOMED Tags fetched: {tags.Count} in {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");


            // Get the csv data into memory
            timer = Stopwatch.StartNew();

            var data = _csvParser.ReadFromFile(
                    Program.Config["SnomedFile"],
                    Encoding.UTF8)
                .ToList();

            _totalRecords = data.Count;

            timer.Stop();
            Console.WriteLine($"SNOMED CSV parsed: {_totalRecords} records in {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");

            // Prep the data
            timer = Stopwatch.StartNew();

            var terms = new Dictionary<string, SnomedTerm>();

            foreach (var term in data)
            {
                if (!term.IsValid)
                {
                    Console.WriteLine($"{term.Error.Value} at column {term.Error.ColumnIndex}");
                    _invalidRecords++;
                    continue;
                }

                var snomed = new SnomedTerm();

                // validate the tag
                var tag = term.Result.SnomedTag.Trim();
                tag = Program.Config[$"TagMappings:{tag}"] ?? tag; // try and remap the tag

                if (!string.IsNullOrEmpty(tag))
                {
                    var snomedTag = tags.SingleOrDefault(x => x.Value == tag);

                    if (snomedTag == null)
                    {
                        Console.WriteLine($"'{tag}' is invalid.");
                        Console.WriteLine("Full Record:");
                        Console.WriteLine(
                            $"\"{term.Result.Id}\"," +
                            $"\"{term.Result.Description}\"," +
                            $"\"{term.Result.SnomedTag}\"");
                        _invalidRecords++;
                        continue;
                    }

                    snomed.SnomedTagId = snomedTag?.Id;
                }

                // Set the simple properties
                snomed.Description = term.Result.Description;
                snomed.Id = term.Result.Id;

                // check for dupes
                if (terms.ContainsKey(snomed.Id))
                {
                    Console.WriteLine($"Duplicate in file: {snomed.Id}");
                    _duplicateRecords++;
                    continue;
                }

                terms[snomed.Id] = snomed;
                _processedRecords++;
            }

            //Report on the processing
            timer.Stop();
            Console.WriteLine($"Snomed Term data prepared: {_processedRecords}");
            Console.WriteLine($"Duplicates: {_duplicateRecords}");
            Console.WriteLine($"Invalid: {_invalidRecords}");
            Console.WriteLine($"Sum of above: {_processedRecords + _duplicateRecords + _invalidRecords}");
            Console.WriteLine($"CSV record count: {_totalRecords}");

            Console.WriteLine($"Preparation took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {Program.Timer.Elapsed}");

            return terms.Values;
        }
    }
}
