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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Do not want any errors for been retuned to the frontend UI for Ajax calls.")]
        public async Task<JsonResult> SuggestLocationsAsync(string term)
        {
            try
            {
                var suggestedResults = await locationService.GetSuggestedLocationsAsync(term).ConfigureAwait(false);
                List<LocationSuggestViewModel> suggestedLocations = suggestedResults.Select(x => new LocationSuggestViewModel
                {
                    Label = $"{x.LocationName} ({x.LocalAuthorityName})",
                    Value = $"{x.Longitude}|{x.Latitude}",
                }).ToList();
                return new JsonResult(suggestedLocations);
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(this.SuggestLocationsAsync)} threw an exception" + ex.Message);
            }

            //if there are any errors return the term that the user has typed.
            return new JsonResult(new List<LocationSuggestViewModel>
            {
                new LocationSuggestViewModel()
                {
                    Label = term,
                    Value = string.Empty,
                },
            });
        }
    }
}
