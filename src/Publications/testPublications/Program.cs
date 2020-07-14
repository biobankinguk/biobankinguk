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

namespace Publications
{
    class Program
    {

        public static async Task Main(string[] args)
        {          
            //Instanitate new instance of getPublication class
            GetPublications getPublications = new GetPublications();
            List<string> obj = await getPublications.GetPublicationItems();
            //Loop through child list to get result list values
            
            /*foreach (var item in obj.Childs.Results)
            {
                Console.WriteLine(item.Id);
                Console.WriteLine(item.Title);
                Console.WriteLine(item.Authors);
                Console.WriteLine(item.Journal);
                Console.WriteLine(item.Year);
                Console.WriteLine(item.Doi);
                Console.WriteLine();
            } */

            foreach (var item in obj)
            {
                Console.WriteLine(item);
            }
            
        }

    }
}

