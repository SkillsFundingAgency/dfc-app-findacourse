using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    [Trait("Category", "Health Controller Unit Tests")]
    public class HealthControllerTests
    {
        private readonly ILogger<HealthController> fakeLogger = A.Fake<ILogger<HealthController>>();
        private readonly IFindACourseService fakeFindACourseService = A.Fake<IFindACourseService>();

        [Fact]
        public void HealthControllerPingReturnsSuccess()
        {
            // Arrange
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            // Act
            var result = controller.Ping();

            // Assert
            var statusResult = Assert.IsType<OkResult>(result);

            A.Equals((int)HttpStatusCode.OK, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task HealthControllerHealthReturnsSuccessWhenHealthy()
        {
            // Arrange
            bool expectedResult = true;
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => fakeFindACourseService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeFindACourseService.PingAsync()).MustHaveHappenedOnceExactly();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var models = Assert.IsAssignableFrom<List<HealthItemViewModel>>(jsonResult.Value);

            models.Count.Should().BeGreaterThan(0);
            models.First().Service.Should().NotBeNullOrWhiteSpace();
            models.First().Message.Should().NotBeNullOrWhiteSpace();

            controller.Dispose();
        }

        [Fact]
        public async Task HealthControllerHealthReturnsServiceUnavailableWhenUnhealthy()
        {
            // Arrange
            bool expectedResult = false;
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => fakeFindACourseService.PingAsync()).Returns(expectedResult);

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeFindACourseService.PingAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);

            controller.Dispose();
        }

        [Fact]
        public async Task HealthControllerHealthReturnsServiceUnavailableWhenException()
        {
            // Arrange
            var controller = BuildHealthController(MediaTypeNames.Application.Json);

            A.CallTo(() => fakeFindACourseService.PingAsync()).Throws<Exception>();

            // Act
            var result = await controller.Health().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeFindACourseService.PingAsync()).MustHaveHappenedOnceExactly();

            var statusResult = Assert.IsType<StatusCodeResult>(result);

            A.Equals((int)HttpStatusCode.ServiceUnavailable, statusResult.StatusCode);

            controller.Dispose();
        }

        private HealthController BuildHealthController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new HealthController(fakeLogger, fakeFindACourseService)
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
