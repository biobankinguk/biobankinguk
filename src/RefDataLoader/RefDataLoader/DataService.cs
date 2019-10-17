﻿using Common.Data.ReferenceData;
using Common.DTO;
using Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RefDataLoader
{
    public class DataService : IDataService
    {
        private readonly ApiSettings _config;
        private readonly HttpClient _client;

        public DataService(IOptions<ApiSettings> options, HttpClient client)
        {
            _config = options.Value;
            _client = client;
        }

        public async Task SeedData()
        {
            foreach(var s in _config.RefDataEndpoints)
            {
                //we don't want the irregular ref data yet (to be handled below)
                var containsIrregularRefData = new List<string> { "AnnualStatistic", "County", "DonorCount", "MaterialType" }.Contains(s.Key, StringComparer.OrdinalIgnoreCase);

                if (!containsIrregularRefData)
                {
                    var refData = PrepData<SortedRefDataBaseDto>($"RefDataSeeding/{s.Key}.json");

                    await SubmitData(refData, s);
                }
            }

            //Once the above is done we can move onto the less standard types, as some of these need Groups to have been seeded first.

            //TODO implement non standard RefData (to be done with relevant PBI due to DTO refactoring)
            //donor count

            await PrepareAndSubmitAnnualStatistics(); 
            

            //county
            //material type
        }

        private async Task PrepareAndSubmitAnnualStatistics()
        {
            var asconfig = _config.RefDataEndpoints.SingleOrDefault(x => x.Key == "AnnualStatistic");

            //get annual statistic groups
            var annualStatisticGroups = await GetRefData<AnnualStatisticGroup>(_config.RefDataEndpoints.SingleOrDefault(x => x.Key == "AnnualStatisticGroup").Value);

            var annualStatistics = new List<AnnualStatisticDto>();

            //match them by name to ones in DTOs, and set the ID values
            foreach(var annualstatistic in PrepData<AnnualStatisticDto>($@"RefDataSeeding/{asconfig.Key}.json"))
            {
                annualstatistic.AnnualStatisticGroupId = annualStatisticGroups.Single(x => x.Value.Contains(annualstatistic.Group, StringComparison.OrdinalIgnoreCase)).Id;
                annualStatistics.Add(annualstatistic);
            }

            await SubmitData(annualStatistics, asconfig);

        }
        private async Task SubmitData<T>(IList<T> data, KeyValuePair<string, string> refDataInfo)
        {
            //We post ontologies individually.

            var timer = Stopwatch.StartNew();
            var count = 1;
            foreach (var refData in data)
            {
                var result = await SendJsonAsync(refDataInfo.Value, JsonConvert.SerializeObject(refData));

                Console.WriteLine(
                    $"Ref Data POST: {refDataInfo.Key}" +
                    $" ({count} / {data.Count}):" +
                    $" Post {(result ? "successful" : "failed")}");
                count++;
            }

            timer.Stop();
            Console.WriteLine($"Ref Data ({refDataInfo.Key}) Posts took: {timer.Elapsed}");
            Console.WriteLine($"Running time so far: {timer.Elapsed}");
        }

        private List<T> PrepData<T>(string datafile)
        {
            using var r = new StreamReader(datafile);
            string json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        private async Task<bool> SendJsonAsync(string endpoint, string data)
        {
            try
            {
                var r = await _client.SendAsync(
                    new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                    //Headers = { Authorization = new AuthenticationHeaderValue("Bearer", Config["BearerToken"]) }, //todo replace with token given by provider
                    RequestUri = new Uri(_client.BaseAddress, endpoint),
                        Content = new StringContent(data, Encoding.UTF8, "application/json")
                    });

                return r.IsSuccessStatusCode;
            }
            catch(Exception e)
            {
                Console.WriteLine("POST failed: " + e.Message); ;
                return false;
            }
        }

        private async Task<List<T>> GetRefData<T>(string endpoint) where T : BaseReferenceDatum
        {
            var r = (await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                //Headers = { Authorization = new AuthenticationHeaderValue("Bearer", Config["BearerToken"]) }, //todo replace with token given by provider
                RequestUri = new Uri(_client.BaseAddress, endpoint)
            })).EnsureSuccessStatusCode();

            var json = await r.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
    }

    public interface IDataService
    {
        Task SeedData();
    }
}
