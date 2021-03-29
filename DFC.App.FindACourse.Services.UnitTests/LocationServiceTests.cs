using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.Services.UnitTests.FakeHttpHandler;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.Services.UnitTests
{
    [Trait("Category", "Location service tests")]
    public class LocationServiceTests
    {
        private readonly ILogger<LocationService> fakeLogger = A.Fake<ILogger<LocationService>>();

        [Fact]
        public async Task GetSuggestedLocationsReturnsLocations()
        {
            //Setup
            var expectedResponse = GetTestResponse();

            var httpResponse = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedResponse)),
                StatusCode = HttpStatusCode.Accepted,
            };

            var fakeHttpRequestSender = A.Fake<IFakeHttpRequestSender>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(fakeHttpRequestSender);
            var httpClient = new HttpClient(fakeHttpMessageHandler);
            httpClient.BaseAddress = new System.Uri("https://dummy.com");

            A.CallTo(() => fakeHttpRequestSender.Send(A<HttpRequestMessage>.Ignored)).Returns(httpResponse);

            var locationService = new LocationService(fakeLogger, httpClient);

            //Act
            var result = await locationService.GetSuggestedLocationsAsync("testTerm").ConfigureAwait(false);

            //Assert
            result.Should().BeEquivalentTo(expectedResponse);

            httpResponse.Dispose();
            httpClient.Dispose();
            fakeHttpMessageHandler.Dispose();
        }

        private List<Location> GetTestResponse()
        {
            return new List<Location>
            {
                new Location()
                {
                    LocationId = "1",
                    LocationName = "LN1",
                    LocalAuthorityName = "LAN1",
                    LocationAuthorityDistrict = "LAD1",
                    Longitude = 1,
                    Latitude = 2,
                },
            };
        }
    }
}