using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;
using Publications;
using Publications.Services;


namespace Publications
{
    class Program
    {

        public static async Task Main(string[] args)
        {
        }

        public static async Task TestEMPCAsync()
        {
            EMPCWebService empcWebService = new EMPCWebService();
            List<Publication> publications = await empcWebService.GetOrganisationPublications("CANDAS");
            Console.WriteLine(publications.Count);
        }
    }
}

