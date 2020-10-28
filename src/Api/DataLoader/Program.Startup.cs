using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Biobanks.DataLoader.Services;
using Microsoft.Extensions.Configuration;

namespace Biobanks.DataLoader
{
    public partial class Program
    {
        private static void Configure(string[] args)
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile("Config/config.json", false)
                .AddCommandLine(args)
                .Build();

            //rebuild config using environment files if env is specified in the command line :)
            if (!string.IsNullOrWhiteSpace(Config["env"]))
            {
                Config = new ConfigurationBuilder()
                    .AddJsonFile("Config/config.json")
                    .AddJsonFile($"Config/config.{Config["env"]}.json", true)
                    .AddCommandLine(args)
                    .Build();
            }
        }

        private static void PrepareHttpClient()
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri(Config["DataServiceUri"])
            };
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private static void InitialiseServices()
        {
            // Order is important as per the comments below.
            // This list is the order processing will be done in.
            Services = new List<IDataLoadService>
            {
                //These are independent and can be done in any order
                new SampleContentMethodService(),
                new SexService(),
                new StorageTemperatureService(),
                new TreatmentLocationService(),
                new OntologyService(),
                new OntologyVersionService(),

                ////SnomedTags are needed before SnomedTerms
                new SnomedTagService(),
                new SnomedTermService(),

                ////MaterialTypeGroups and SnomedTerms are needed before MaterialTypes
                new MaterialTypeGroupService(),
                new MaterialTypeService()
            };
        }
    }
}