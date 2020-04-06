using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.CompositeInterfaceModels.FindACourseClient;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    public class CourseControllerTests : BaseController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerIndexReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<IndexViewModel>(viewResult.ViewData.Model);

            Assert.NotNull(model.Documents);
            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerHeadReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = controller.Head();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(viewResult.ViewData.Model);

            Assert.NotNull(model);
            Assert.Equal("Find a Course", model.Title);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerHeroBannerReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = controller.HeroBanner(string.Empty);

            Assert.IsType<ViewResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerBreadcrumbHtmlReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = controller.Breadcrumb();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BreadcrumbViewModel>(viewResult.ViewData.Model);

            model.Paths.Count.Should().BeGreaterThan(0);
            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerBodyTopReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = controller.BodyTop();

            Assert.IsType<ViewResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public void CourseControllerDocumentReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = controller.Document();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);

            Assert.Equal("Find a Course", model.Title);
            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public void CourseControllerBodyFooterReturnsNoContent(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = controller.BodyFooter();

            Assert.IsType<NoContentResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerFilterResultsReturnsSuccess(string mediaTypeName)
        {
            var courseService = A.Fake<IFindACourseService>();
            var logger = A.Fake<ILogger<CourseController>>();
            var controller = BuildCourseController(mediaTypeName);
            var bodyViewModel = new BodyViewModel
            {
                CurrentSearchTerm = "Maths",
                SideBar = new SideBarViewModel(),
            };

            var returnedCourseData = new CourseSearchResult
            {
                Courses = new List<Course>
                {
                    new Course { Title = "Maths", CourseId = "1", AttendancePattern = "Online" },
                },
            };

            A.CallTo(() => courseService.GetFilteredData(A<CourseSearchFilters>.Ignored, CourseSearchOrderBy.Relevance, 1)).Returns(returnedCourseData);

            var result = await controller.FilterResults(bodyViewModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);

            controller.Dispose();
        }
    }
}
