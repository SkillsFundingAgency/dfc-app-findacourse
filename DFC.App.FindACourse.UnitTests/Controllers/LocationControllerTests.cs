using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;
using FakeItEasy;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using DFC.App.FindACourse.Data.Models;
using Newtonsoft.Json;
using System.Linq;
using FluentAssertions;
using System.Net;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    [Trait("Category", "Location Controller Unit Tests")]
    public class LocationControllerTests
    {
        private readonly ILogService fakeLogService;

        private readonly ILocationService fakeLocationsService;

        public LocationControllerTests()
        {
            fakeLocationsService = A.Fake<ILocationService>();
            fakeLogService = A.Fake<ILogService>();
        }

        [Fact]
        public async Task SuggestLocationsAsyncReturnsSuccess()
        {
            // arrange
            var suggestedLocations = GetTestSuggestedLocations();
            var testLocation = suggestedLocations.FirstOrDefault();
            var controller = BuildLocationController(MediaTypeNames.Text.Html);

            A.CallTo(() => fakeLocationsService.GetSuggestedLocationsAsync(A<string>.Ignored)).Returns(GetTestSuggestedLocations());

            // act
            var jsonResult = await controller.SuggestLocationsAsync("testTerm").ConfigureAwait(false);

            // assets
            var resultObject = Assert.IsType<JsonResult>(jsonResult);
            var resultList = resultObject.Value as IEnumerable<LocationSuggestViewModel>;

            resultList.Count().Should().Be(1);

            resultList.FirstOrDefault().Label.Should().Be($"{testLocation.LocationName} ({testLocation.LocalAuthorityName})");
            resultList.FirstOrDefault().Value.Should().Be($"{testLocation.Longitude}|{testLocation.Latitude}");

            controller.Dispose();
        }

        private IEnumerable<Location> GetTestSuggestedLocations()
        {
            yield return new Location()
            {
                LocationId = "123",
                LocationName = "LN1",
                LocalAuthorityName = "LAN1",
                LocationAuthorityDistrict = "LAD1",
                Longitude = 1.23,
                Latitude = 4.56,
            };
        }

        private LocationController BuildLocationController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new LocationController(fakeLogService, fakeLocationsService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}
