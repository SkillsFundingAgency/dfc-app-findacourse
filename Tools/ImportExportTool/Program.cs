using Azure;
using Azure.Search.Documents;
using ConsoleApp3.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using CsvHelper;

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
            string fileType;
            
            switch (action)
            {
                case "import":
                    Console.Write("File name (must be accessible in the Data folder): ");
                    var fileName = Console.ReadLine();
                    Console.WriteLine(string.Empty);

                    Console.Write("File type (onsjson or csv): ");
                    fileType = Console.ReadLine();
                    Console.WriteLine(string.Empty);
                    
                    Console.Write("Are you sure? This will add to the index - even if it has data in it. Type 'yes' if you wish to continue: ");
                    var sure = Console.ReadLine().ToLower();

                    if (!sure.Equals("yes"))
                    {
                        Console.WriteLine("Import cancelled");
                        break;
                    }

                    Import(searchClient, fileName, fileType);
                    break;
                case "export":
                    Console.Write("File type (json or csv): ");
                    fileType = Console.ReadLine();
                    Console.WriteLine(string.Empty);
                    
                    Export(searchClient, fileType);
                    break;
                default:
                    Console.WriteLine($"'{action}' is not a recognised action");
                    break;
            }
        }

        static void Export(SearchClient searchClient, string fileType)
        {
            var response = searchClient.Search<ImportLocation>("*", new SearchOptions { Size = 50000 });
            var results = response.Value.GetResults();
            var output = new List<ImportLocation>();

            foreach (var result in results)
            {
                output.Add(result.Document);
            }

            if (fileType.Equals("json", StringComparison.InvariantCultureIgnoreCase))
            {
                var outputText = JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($"{DateTime.Now.ToString("yyyy-MM-dd-hhmmss")}-ExportedData.json", outputText);                
            }
            else
            {
                using var stream = new MemoryStream();
                using var reader = new StreamReader( stream );
                using var writer = new StreamWriter( stream );
                using var csv = new CsvWriter(writer, CultureInfo.CurrentCulture);

                csv.WriteRecords(output);
                writer.Flush();
                stream.Position = 0;

                var text = reader.ReadToEnd();
                File.WriteAllText($"{DateTime.Now.ToString("yyyy-MM-dd-hhmmss")}-ExportedData.csv", text);
            }
        }

        static void Import(SearchClient searchClient, string dataFile, string fileType)
        {
            using var streamReader = new StreamReader($"Data/{dataFile}");
            List<InputLocation> inputItems;
            
            switch (fileType.ToLower())
            {
                case "onsjson":
                    var jsonDataString = streamReader.ReadToEnd();

                    inputItems = JsonSerializer.Deserialize<List<InputLocation>>(jsonDataString)
                        .Where(x => x.type != "Hamlet")
                        .ToList();
                    break;
                case "csv":
                    inputItems = ReadCsv(streamReader)
                        .ToList();
                    break;
                default:
                    throw new Exception($"File type {fileType} not known");
            }

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

        static IEnumerable<InputLocation> ReadCsv(StreamReader reader)
        {
            using var csv = new CsvReader(reader, CultureInfo.CurrentCulture, true);
            var records = csv.GetRecords<ImportLocation>();
            return records.Select(record => new InputLocation
            {  
                county = record.LocalAuthorityName,
                id = long.Parse(record.LocationId),
                latitude = record.Latitude,
                local_government_area = record.LocationAuthorityDistrict,
                longitude = record.Longitude,
                name = record.LocationName,
                type = "Unknown"
            }).ToList();
        }

        static IEnumerable<IEnumerable<TValue>> Chunk<TValue>(IEnumerable<TValue> values, int chunkSize)
        {
            return values
                .Select((v, i) => new { v, groupIndex = i / chunkSize })
                .GroupBy(x => x.groupIndex)
                .Select(g => g.Select(x => x.v));
        }
    }
}
