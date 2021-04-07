using Azure;
using Azure.Search.Documents;
using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    [ExcludeFromCodeCoverage]
    public class LocationService : ILocationService
    {
        private readonly ILogger<LocationService> logger;
        private readonly AzureSearchIndexConfig azureSearchIndexConfig;
        private readonly string suggestorName = "sglocation";
        private SearchClient searchClient;
        private SuggestOptions suggestOptions;

        public LocationService(ILogger<LocationService> logger, AzureSearchIndexConfig azureSearchIndexConfig)
        {
            this.logger = logger;
            this.azureSearchIndexConfig = azureSearchIndexConfig;
            CreateSearchClient();
        }

        public async Task<IEnumerable<SearchLocationIndex>> GetSuggestedLocationsAsync(string term)
        {
            logger.LogInformation($"Making request to search index for term {term}");
            try
            {
                var suggestResults = await this.searchClient.SuggestAsync<SearchLocationIndex>(term, suggestorName, this.suggestOptions).ConfigureAwait(false);
                var documents = suggestResults.Value.Results.Select(i => i.Document);
                logger.LogInformation($"Returning location results for term {term}");
                return documents;
            }
            catch (Exception ex)
            {
                logger.LogError("Getting suggestions had an error", ex);
                throw;
            }
        }

        private void CreateSearchClient()
        {
            this.suggestOptions = new SuggestOptions()
            {
                Size = 20,
            };

            suggestOptions.Select.Add(nameof(SearchLocationIndex.LocationId));
            suggestOptions.Select.Add(nameof(SearchLocationIndex.LocationName));
            suggestOptions.Select.Add(nameof(SearchLocationIndex.LocationAuthorityDistrict));
            suggestOptions.Select.Add(nameof(SearchLocationIndex.LocalAuthorityName));
            suggestOptions.Select.Add(nameof(SearchLocationIndex.Longitude));
            suggestOptions.Select.Add(nameof(SearchLocationIndex.Latitude));

            var azureKeyCredential = new AzureKeyCredential(azureSearchIndexConfig.SearchServiceAdminAPIKey);
            this.searchClient = new SearchClient(azureSearchIndexConfig.EndpointUri, azureSearchIndexConfig.LocationSearchIndex, azureKeyCredential);
        }
    }
}
