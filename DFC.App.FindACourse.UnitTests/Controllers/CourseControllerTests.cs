using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.CompositeInterfaceModels.FindACourseClient;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    public class CourseControllerTests : BaseController
    {
        private readonly IFindACourseService fakefindACourseService = A.Fake<IFindACourseService>();

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
        public void CourseControllerHeadReturnsSuccess(string mediaTypeName)
        {
            // arrange
            var controller = BuildCourseController(mediaTypeName);

            // act
            var result = controller.Head(string.Empty);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(viewResult.ViewData.Model);

            Assert.NotNull(model);
            Assert.Equal("Results | Find a course | National Careers Service", model.Title);

            controller.Dispose();
        }

        [Theory]
        [InlineData("course", "Results")]
        [InlineData("details", "Details")]
        [InlineData("", "Results")]
        public void CourseControllerHeadReturnsTitleSuccess(string articleName, string expectedResultStub)
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);

            // act
            var result = controller.Head(articleName);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<HeadViewModel>(viewResult.ViewData.Model);

            Assert.NotNull(model);
            Assert.Equal($"{expectedResultStub} | Find a course | National Careers Service", model.Title);

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
        public async Task CourseControllerBodyReturnsNoContent(string mediaTypeName)
        {
            // arrange
            var controller = BuildCourseController(mediaTypeName);
            var dummyCourseSearchResult = A.Dummy<CourseSearchResult>();

            A.CallTo(() => fakefindACourseService.GetFilteredData(A<CourseSearchFilters>.Ignored, A<CourseSearchOrderBy>.Ignored, A<int>.Ignored)).Returns(dummyCourseSearchResult);

            // act
            var result = await controller.Body("course").ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            Assert.NotNull(model.Results);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public void CourseControllerBodyFooterReturnsNoContent(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);

            var result = controller.BodyFooter("course");

            Assert.IsType<NoContentResult>(result);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerFilterResultsReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildCourseController(mediaTypeName);
            var bodyViewModel = new BodyViewModel
            {
                CurrentSearchTerm = "Maths",
                SideBar = new SideBarViewModel
                {
                    DistanceValue = "15 miles",
                    CourseType = new FiltersListViewModel
                    {
                        SelectedIds = new List<string> { "Online" },
                    },
                    CourseHours = new FiltersListViewModel
                    {
                        SelectedIds = new List<string> { "Full time" },
                    },
                    CourseStudyTime = new FiltersListViewModel
                    {
                        SelectedIds = new List<string> { "Daytime" },
                    },
                },
                IsTest = true,
            };

            var returnedCourseData = new CourseSearchResult
            {
                Courses = new List<Course>
                {
                    new Course { Title = "Maths", CourseId = "1", AttendancePattern = "Online", Description = "This is a test description - over 220 chars" + new string(' ', 220) },
                },
            };

            A.CallTo(() => FakeFindACoursesService.GetFilteredData(A<CourseSearchFilters>.Ignored, CourseSearchOrderBy.Relevance, 1)).Returns(returnedCourseData);

            var result = await controller.FilterResults(bodyViewModel).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);

            controller.Dispose();
        }

        [Theory]
        [InlineData("CV1 2WT")]
        [InlineData("Coventry")]
        [InlineData("")]
        public async Task PageReturnsSuccess(string town)
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);
            var paramValues = new ParamValues
            {
                Town = town,
                CourseType = "Online",
                CourseHours = "Full time",
                CourseStudyTime = "Daytime",
            };
            var returnedCourseData = new CourseSearchResult
            {
                Courses = new List<Course>
                {
                    new Course { Title = "Maths", CourseId = "1", AttendancePattern = "Online", Description = "This is a test description - over 220 chars" + new string(' ', 220) },
                },
            };

            A.CallTo(() => FakeFindACoursesService.GetFilteredData(A<CourseSearchFilters>.Ignored, CourseSearchOrderBy.Relevance, 1)).Returns(returnedCourseData);

            // act
            var result = await controller.Page(paramValues, true).ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);
            Assert.NotNull(model.Results);

            controller.Dispose();
        }

        [Fact]
        public async Task AjaxChangedReturnsSuccess()
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);
            var paramValues = new ParamValues
            {
                Town = "CV1 2WT",
                CourseType = "Online",
                CourseHours = "Full time",
                CourseStudyTime = "Daytime",
            };
            var appdata = System.Text.Json.JsonSerializer.Serialize(paramValues);
            var returnedCourseData = new CourseSearchResult
            {
                ResultProperties = new CourseSearchResultProperties
                {
                    Page = 1,
                    TotalResultCount = 1,
                    TotalPages = 1,
                },
                Courses = new List<Course>
                {
                    new Course { Title = "Maths", CourseId = "1", AttendancePattern = "Online", Description = "This is a test description - over 220 chars" + new string(' ', 220) },
                },
            };

            A.CallTo(() => FakeFindACoursesService.GetFilteredData(A<CourseSearchFilters>.Ignored, A<CourseSearchOrderBy>.Ignored, A<int>.Ignored)).Returns(returnedCourseData);
            A.CallTo(() => FakeViewHelper.RenderViewAsync(A<CourseController>.Ignored, A<string>.Ignored, A<BodyViewModel>.Ignored, A<bool>.Ignored)).Returns("<p>some markup</p>");

            // act
            var result = await controller.AjaxChanged(appdata).ConfigureAwait(false);

            // assert
            Assert.False(string.IsNullOrWhiteSpace(result.HTML));
            Assert.Equal(returnedCourseData.Courses.ToList().Count, result.Count);
            Assert.True(result.IsPostcode);

            controller.Dispose();
        }

        [Fact]
        public async Task AjaxChangedCatchesException()
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);
            var paramValues = new ParamValues
            {
                Town = "CV1 2WT",
                CourseType = "Online",
                CourseHours = "Full time",
                CourseStudyTime = "Daytime",
            };
            var appdata = System.Text.Json.JsonSerializer.Serialize(paramValues);

            A.CallTo(() => FakeFindACoursesService.GetFilteredData(A<CourseSearchFilters>.Ignored, A<CourseSearchOrderBy>.Ignored, A<int>.Ignored)).Throws(new Exception());
            A.CallTo(() => FakeViewHelper.RenderViewAsync(A<CourseController>.Ignored, A<string>.Ignored, A<BodyViewModel>.Ignored, A<bool>.Ignored)).Returns("<p>some markup</p>");

            // act
            var result = await controller.AjaxChanged(appdata).ConfigureAwait(false);

            // assert
            Assert.False(string.IsNullOrWhiteSpace(result.HTML));
            Assert.Equal(0, result.Count);
            Assert.Null(result.IsPostcode);

            controller.Dispose();
        }

        [Fact]
        public async Task AjaxChangedReturnsArgumentNullExceptionForMissingParameters()
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);

            // act
            Func<Task> act = async () => await controller.AjaxChanged(null).ConfigureAwait(false);

            // assert
            act.Should().Throw<ArgumentNullException>();
            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task CourseControllerFilterResultsThrowsException(string mediaTypeName)
        {
            // arrange
            var controller = BuildCourseController(mediaTypeName);
            var bodyViewModel = new BodyViewModel
            {
                CurrentSearchTerm = "Maths",
                SideBar = new SideBarViewModel(),
                IsTest = true,
            };

            A.CallTo(() => FakeFindACoursesService.GetFilteredData(A<CourseSearchFilters>.Ignored, CourseSearchOrderBy.Relevance, 1)).Throws(new Exception());

            // act
            var result = await controller.FilterResults(bodyViewModel).ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);
            Assert.Null(model.Results);

            controller.Dispose();
        }

        [Fact]
        public async Task CourseControllerSearchCourseThrowsException()
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);

            A.CallTo(() => FakeFindACoursesService.GetFilteredData(A<CourseSearchFilters>.Ignored, A<CourseSearchOrderBy>.Ignored, A<int>.Ignored)).Throws(new Exception());

            // act
            var result = await controller.SearchCourse("search term").ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            Assert.Null(model.Results);

            controller.Dispose();
        }

        [Theory]
        [InlineData("\"CV1 2WT\"", true)]
        [InlineData("\"Coventry\"", false)]
        public async Task CourseControllerIsValidPostcodeReturnsSuccess(string town, bool expectedResult)
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);

            // act
            var result = await controller.IsValidPostcode(town).ConfigureAwait(false);

            // assert
            Assert.Equal(expectedResult, result);

            controller.Dispose();
        }

        [Fact]
        public async Task CourseControllerIsValidPostcodeThrowsException()
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);

            // act
            Func<Task> act = async () => await controller.IsValidPostcode("\"\"").ConfigureAwait(false);

            // assert
            act.Should().Throw<ArgumentNullException>();

            controller.Dispose();
        }
    }
}
