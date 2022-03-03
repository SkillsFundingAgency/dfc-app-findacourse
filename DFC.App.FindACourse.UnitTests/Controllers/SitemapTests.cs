using DFC.App.FindACourse.Controllers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    public class SitemapTests
    {
        private readonly SitemapController controller;

        public SitemapTests()
        {
            controller = new SitemapController(A.Fake<ILogger<SitemapController>>());
        }

        [Fact]
        public void SitemapGeneratedSuccesfully()
        {
            var actionResponse = controller.SitemapView();
            Assert.IsType<ContentResult>(actionResponse);
        }
    }
}
