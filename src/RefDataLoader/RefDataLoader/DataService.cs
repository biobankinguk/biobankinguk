using Common.Data.ReferenceData;
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
            foreach (var s in _config.RefDataEndpoints)
            {
                //we don't want the irregular ref data yet (to be handled below)
                var containsIrregularRefData = new List<string> { "AnnualStatistic", "County", "DonorCount", "MaterialType" }.Contains(s.Key, StringComparer.OrdinalIgnoreCase);

                if (!containsIrregularRefData)
                {
                    var refData = PrepData<SortedRefDataBaseDto>($"RefDataSeeding/{s.Key}.json");

                    await SubmitData(refData, s);
                }
            }

            //Once the above is done we can move onto the less standard types, as some of these need Groups or other entities to have been seeded first.
                        
            await PrepareAndSubmitAnnualStatistics();
            await PrepareAndSubmitMaterialType();
            await PrepareAndSubmitCounty();
            await PrepareAndSubmitDonorCount();
        }

        private async Task PrepareAndSubmitDonorCount()
        {
            var donorCountConfig = _config.RefDataEndpoints.SingleOrDefault(x => x.Key == "DonorCount");
            var donorCounts = PrepData<DonorCountDto>($"RefDataSeeding/{donorCountConfig.Key}.json");
            await SubmitData(donorCounts, donorCountConfig);
        }

        private async Task PrepareAndSubmitCounty()
        {
            var countyConfig = _config.RefDataEndpoints.SingleOrDefault(x => x.Key == "County");

            //get countries
            var countries = await GetRefData<Country>(_config.RefDataEndpoints.SingleOrDefault(x => x.Key == "Country").Value);
            var counties = new List<CountyDto>();

            //match them by name to ones in DTOs, and set the ID values
            foreach(var county in PrepData<CountyDto>($"RefDataSeeding/{countyConfig.Key}.json"))
            {
                county.CountryId = countries.Single(x => x.Value.Equals(county.CountryName, StringComparison.OrdinalIgnoreCase)).Id;
                counties.Add(county);
            }

            await SubmitData(counties, countyConfig);
        }

        private async Task PrepareAndSubmitAnnualStatistics()
        {
            var asconfig = _config.RefDataEndpoints.SingleOrDefault(x => x.Key == "AnnualStatistic");

            //get annual statistic groups
            var annualStatisticGroups = await GetRefData<AnnualStatisticGroup>(_config.RefDataEndpoints.SingleOrDefault(x => x.Key == "AnnualStatisticGroup").Value);

            var annualStatistics = new List<AnnualStatisticDto>();

            //match them by name to ones in DTOs, and set the ID values
            foreach(var annualstatistic in PrepData<AnnualStatisticDto>($"RefDataSeeding/{asconfig.Key}.json"))
            {
                annualstatistic.AnnualStatisticGroupId = annualStatisticGroups.Single(x => x.Value.Equals(annualstatistic.Group, StringComparison.OrdinalIgnoreCase)).Id;
                annualStatistics.Add(annualstatistic);
            }

            await SubmitData(annualStatistics, asconfig);
        }

        private async Task PrepareAndSubmitMaterialType()
        {
            var mtConfig = _config.RefDataEndpoints.SingleOrDefault(x => x.Key == "MaterialType");

            // Get the material type groups
            var materialTypeGroups = await GetRefData<MaterialTypeGroup>(_config.RefDataEndpoints.SingleOrDefault(x => x.Key == "MaterialTypeGroup").Value);

            var materialTypes = new List<MaterialTypeDto>();
            try
            {
                var materialTypeData = PrepData<MaterialTypeDto>($"RefDataSeeding/{mtConfig.Key}.json");
                // match them by name to ones in DTOs, and set the ID values
                foreach (var materialType in materialTypeData)
                {
                    foreach (var group in materialType.MaterialTypeGroups)
                    {
                        //var x = materialTypeGroups.Where(x => x.Value.Contains(group.GroupName, StringComparison.OrdinalIgnoreCase)).ToList();
                        group.GroupId = materialTypeGroups.SingleOrDefault(x => x.Value.Equals(group.GroupName, StringComparison.OrdinalIgnoreCase)).Id;
                    }
                    materialTypes.Add(materialType);
                    Console.WriteLine($"{materialType.Value} added.");
                }
            }
            catch(Exception e)
            {
                throw;
            }
            await SubmitData(materialTypes, mtConfig);
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
                Console.WriteLine("POST failed: " + e.Message);
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
