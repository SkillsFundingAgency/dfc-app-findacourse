using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.FindACourseClient;
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

            var result = await controller.Index().ConfigureAwait(false);

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

            var result = await controller.Head().ConfigureAwait(false);

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

            var result = await controller.HeroBanner().ConfigureAwait(false);

            Assert.IsType<ViewResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerBreadcrumbHtmlReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = await controller.Breadcrumb().ConfigureAwait(false);

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

            var result = await controller.BodyTop().ConfigureAwait(false);

            Assert.IsType<ViewResult>(result);
 
            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerDocumentReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = await controller.Document(string.Empty).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<DocumentViewModel>(viewResult.ViewData.Model);

            Assert.Equal("Find a Course", model.Title);
            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerBodyFooterReturnsNoContent(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = await controller.BodyFooter(string.Empty).ConfigureAwait(false);

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
            var bodyViewModel = new BodyViewModel();
            bodyViewModel.CurrentSearchTerm = "Maths";
            bodyViewModel.SideBar = new SideBarViewModel();

            var returnedCourseData = new CourseSearchResult();
            returnedCourseData.Courses = new List<Course>
            {
                new Course { Title= "Maths", CourseId = "1", AttendancePattern = "Online"}
            };

            A.CallTo(() => courseService.GetFilteredData(A<CourseSearchFilters>.Ignored,CourseSearchOrderBy.Relevance, 1)).Returns(returnedCourseData);

            var result = await controller.FilterResults(bodyViewModel).ConfigureAwait(false);
       
            var viewResult = Assert.IsType<ViewResult>(result);
           
            controller.Dispose();
        }
    }
}
