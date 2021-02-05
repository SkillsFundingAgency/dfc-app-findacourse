using DFC.App.FindACourse.Controllers;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    [Trait("Category", "Robot Controller Unit Tests")]
    public class RobotControllerRobotTests
    {
        private readonly ILogger<RobotController> fakeLogger = A.Fake<ILogger<RobotController>>();
        private readonly IWebHostEnvironment fakeHostingEnvironment = A.Fake<IWebHostEnvironment>();

        [Fact]
        public void RobotControllerRobotReturnsSuccess()
        {
            // Arrange
            var controller = BuildRobotController();

            // Act
            var result = controller.Robot();

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);

            contentResult.ContentType.Should().Be(MediaTypeNames.Text.Plain);

            controller.Dispose();
        }

        private RobotController BuildRobotController()
        {
            var controller = new RobotController(fakeLogger, fakeHostingEnvironment)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext(),
                },
            };

            return controller;
        }
    }
}
