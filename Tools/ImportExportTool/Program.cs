using Azure;
using Azure.Search.Documents;
using ConsoleApp3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Do you wish to import or export? Please choose by typing either word: ");
            var action = Console.ReadLine().ToLower();
            Console.WriteLine(string.Empty);

            Console.Write("Endpoint (for example 'https://something.search.windows.net'): ");
            var searchServiceEndPoint = Console.ReadLine();
            Console.WriteLine(string.Empty);

            Console.Write("API Key (for example '1A428EFFFE67329934C5775DB7E6C091') - must be admin to write: ");
            var apiKey = Console.ReadLine();
            Console.WriteLine(string.Empty);

            Console.Write("Index name (for example 'dfc-digital-locations'): ");
            var indexName = Console.ReadLine();
            Console.WriteLine(string.Empty);

            var searchClient = new SearchClient(new Uri(searchServiceEndPoint), indexName, new AzureKeyCredential(apiKey));

            switch (action)
            {
                case "import":
                    Console.Write("Are you sure? This will add to the index - even if it has data in it. Type 'yes' if you wish to continue: ");
                    var sure = Console.ReadLine().ToLower();

                    if (!sure.Equals("yes"))
                    {
                        Console.WriteLine("Import cancelled");
                        break;
                    }

                    Import(searchClient);
                    break;
                case "export":
                    Export(searchClient);
                    break;
                default:
                    Console.WriteLine($"'{action}' is not a recoginised action");
                    break;
            }
        }

        static void Export(SearchClient searchClient)
        {
            var response = searchClient.Search<ImportLocation>("*", new SearchOptions { Size = 50000 });
            var results = response.Value.GetResults();
            var output = new List<ImportLocation>();

            foreach (var result in results)
            {
                output.Add(result.Document);
            }

            var outputText = JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText($"ExportedData-{DateTime.Now.ToString("yyyyMMdd-hhmmss")}", outputText);
        }

        static void Import(SearchClient searchClient)
        {
            using var streamReader = new StreamReader("Data/england_towns.json");
            var jsonDataString = streamReader.ReadToEnd();
            var inputItems = JsonSerializer.Deserialize<List<InputLocation>>(jsonDataString).Where(x => x.type != "Hamlet").ToList();
            var outputItems = new List<ImportLocation>();

            foreach (var inputItem in inputItems)
            {
                var outputItem = new ImportLocation
                {
                    LocationId = inputItem.id.ToString(),
                    LocationName = inputItem.name,
                    Latitude = inputItem.latitude,
                    Longitude = inputItem.longitude,
                    LocalAuthorityName = inputItem.county,
                    LocationAuthorityDistrict = inputItem.local_government_area
                };

                outputItems.Add(outputItem);
            }

            var batches = Chunk(outputItems, 1000);

            foreach (var batch in batches)
            {
                searchClient.UploadDocuments(batch);
            }
        }

        public static IEnumerable<IEnumerable<TValue>> Chunk<TValue>(IEnumerable<TValue> values, int chunkSize)
        {
            return values
                .Select((v, i) => new { v, groupIndex = i / chunkSize })
                .GroupBy(x => x.groupIndex)
                .Select(g => g.Select(x => x.v));
        }
    }
}
