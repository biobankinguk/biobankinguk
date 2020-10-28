using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Biobanks.DataLoader.Services;
using Microsoft.Extensions.Configuration;

namespace Biobanks.DataLoader
{
    public partial class Program
    {
        public static IConfigurationRoot Config;

        public static HttpClient Client;

        public static ICollection<IDataLoadService> Services;

        public static Stopwatch Timer { get; } = new Stopwatch();

        public static int BatchSize =>
            int.TryParse(Config["BatchSize"], out var result)
                ? result
                : 1000;

        static void Main(string[] args)
        {
            Configure(args);

            PrepareHttpClient();

            InitialiseServices();

            Console.WriteLine($"Data Service URI: {Config["DataServiceUri"]}");
            Console.WriteLine($"SNOMED CSV Path: {Config["SnomedFile"]}");
            Console.WriteLine("Press a key to start data load...");
            Console.Read();

            Timer.Start();

            try
            {
                MainAsync(args).Wait();
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Press any key to exit.");
            Console.Read();

            Console.Read(); // For some reason, the app auto exits if there's not an additional Console.Read()
        }

        static async Task MainAsync(string[] args)
        {
            foreach (var service in Services)
            {
                await service.LoadData();
            }

            Timer.Stop();
            Console.WriteLine($"Data loading complete after {Timer.Elapsed}");
        }
    }
}