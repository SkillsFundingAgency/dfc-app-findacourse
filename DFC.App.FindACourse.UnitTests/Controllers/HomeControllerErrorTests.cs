using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.Mime;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    [Trait("Category", "Home Controller Unit Tests")]
    public class HomeControllerErrorTests
    {
        [Fact]
        public void HomeControllerErrorTestsReturnsSuccess()
        {
            // Arrange
            var controller = BuildHomeController(MediaTypeNames.Text.Html);

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            _ = Assert.IsAssignableFrom<ErrorViewModel>(viewResult.ViewData.Model);

            controller.Dispose();
        }

        private HomeController BuildHomeController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new HomeController()
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
