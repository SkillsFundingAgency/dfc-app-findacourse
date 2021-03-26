using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILogService logService;
        private readonly ILocationService locationService;

        public LocationController(ILogService logService, ILocationService locationService)
        {
            this.logService = logService;
            this.locationService = locationService;
        }

        [HttpGet]
        [Route("api/get/find-a-course/suggestlocationsasync/{term}")]
        public async Task<JsonResult> SuggestLocationsAsync(string term)
        {
            var startTime = DateTime.Now;
            try
            {
                var suggestedResults = await locationService.GetSuggestedLocationsAsync(term).ConfigureAwait(false);
                List<LocationSuggestViewModel> suggestedLocations = suggestedResults.Select(x => new LocationSuggestViewModel
                {
                    Label = $"{x.LocationName} ({x.LocalAuthorityName})",
                    Value = $"{x.Longitude}|{x.Latitude}",
                }).ToList();

                //var msg = $"location request took {(DateTime.Now - startTime).TotalSeconds} seconds";
                //logService.LogInformation(msg);
                return new JsonResult(suggestedLocations);
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(this.SuggestLocationsAsync)} threw an exception" + ex.Message);
                throw;
            }
        }
    }
}
