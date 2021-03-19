using DFC.App.FindACourse.Data.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILogger<LocationService> logger;
        private readonly HttpClient httpClient;

        public LocationService(ILogger<LocationService> logger, HttpClient httpClient)
        {
            this.logger = logger;
            this.httpClient = httpClient;
        }

        public async Task<IEnumerable<Location>> GetSuggestedLocationsAsync(string term)
        {
            var requestUri = new Uri($"{httpClient.BaseAddress}/{term}");

            logger.LogInformation($"Making request to {requestUri}");

            var response = await httpClient.GetAsync(requestUri).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var locationsReturned = JsonConvert.DeserializeObject<IEnumerable<Location>>(jsonResponse);

            logger.LogInformation($"Suggest API returned {locationsReturned.Count()}");

            return locationsReturned;
        }
    }
}
