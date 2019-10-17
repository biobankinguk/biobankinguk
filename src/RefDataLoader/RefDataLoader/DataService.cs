﻿using Common.DTO;
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
                    var refData = PrepData($"RefDataSeeding/{s.Key}.json");

                    await SubmitData(refData, s);
                }
            }

            //TODO implement non standard RefData (to be done with relevant PBI due to DTO refactoring)
            //donor count

            //TODO now handle the non standard objects/ones which have dependencies on groups
            //annualStatistic
            //county
            //material type
        }

        private async Task SubmitData(IList<SortedRefDataBaseDto> data, KeyValuePair<string, string> refDataInfo)
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

        private List<SortedRefDataBaseDto> PrepData(string datafile)
        {
            using var r = new StreamReader(datafile);
            string json = r.ReadToEnd();
            return JsonConvert.DeserializeObject<List<SortedRefDataBaseDto>>(json);
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

        private async Task<string> GetJsonAsync(string endpoint)
        {
            var r = (await _client.SendAsync(new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                //Headers = { Authorization = new AuthenticationHeaderValue("Bearer", Config["BearerToken"]) }, //todo replace with token given by provider
                RequestUri = new Uri(_client.BaseAddress, endpoint)
            })).EnsureSuccessStatusCode();

            return await r.Content.ReadAsStringAsync();
        }
    }

    public interface IDataService
    {
        Task SeedData();
    }
}
