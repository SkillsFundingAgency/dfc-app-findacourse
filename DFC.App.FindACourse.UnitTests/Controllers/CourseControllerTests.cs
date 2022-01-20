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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    [ExcludeFromCodeCoverage]
    public class CourseControllerTests : BaseController
    {
        private readonly IFindACourseService fakefindACourseService = A.Fake<IFindACourseService>();

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public void CourseControllerIndexReturnsSuccess(string mediaTypeName)
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
        public void CourseControllerBreadcrumbHtmlReturnsSuccess(string mediaTypeName)
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

        [Fact]
        public async Task CourseControllerSearchFreeCourseReturnsNoContent()
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);
            var dummyCourseSearchResult = A.Dummy<CourseSearchResult>();

            A.CallTo(() => FakeFindACoursesService.GetFilteredData(A<CourseSearchFilters>._, A<CourseSearchOrderBy>._, A<int>._)).Returns(dummyCourseSearchResult);

            // act
            var result = await controller.SearchFreeCourse("course").ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);

            A.CallTo(() =>
                    FakeFindACoursesService.GetFilteredData(A<CourseSearchFilters>.That.Matches(x => x.CampaignCode == CourseController.FreeSearchCampaignCode), A<CourseSearchOrderBy>._, A<int>._))
                .MustHaveHappenedOnceExactly();

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
                    QualificationLevels = new FiltersListViewModel
                    {
                        SelectedIds = new List<string> { "1" },
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

            var result = await controller.FilterResults(bodyViewModel, string.Empty).ConfigureAwait(false);

            Assert.IsType<ViewResult>(result);

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
        public async Task PageSetsFreeCourseSearchWhenParameterPassedIn()
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);
            var paramValues = new ParamValues
            {
                Town = "town",
                CourseType = "Online",
                CourseHours = "Full time",
                CourseStudyTime = "Daytime",
                CampaignCode = CourseController.FreeSearchCampaignCode,
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
            Assert.True(model.FreeCourseSearch);
            Assert.Equal(model.CourseSearchFilters.CampaignCode, CourseController.FreeSearchCampaignCode);
            Assert.NotNull(model.Results);

            controller.Dispose();
        }

        [Fact]
        public async Task PageReturnsNotFoundForMissingParameters()
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);

            // act
            var actual = await controller.Page(null, false);

            // assert
            actual.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.NotFound));
            controller.Dispose();
        }

        [Theory]
        [InlineData("CV1 2WT", null, true)]
        [InlineData("TestTown", null, false)]
        [InlineData("TownWithCords", "1.23|3.45", true)]
        [InlineData("TownWithInvalidCords", "invalid|3.45", false)]
        [InlineData("TownWithInvalidCords", "1.23|invalid", false)]
        [InlineData("TownWithInvalidCords", "invalid", false)]
        public async Task AjaxChangedReturnsSuccessWithCorrectStates(string townOrPostcode, string coordinates, bool expectedShowDistance)
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);
            var paramValues = new ParamValues
            {
                Town = townOrPostcode,
                Coordinates = coordinates,
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
            result.ShowDistanceSelector.Should().Be(expectedShowDistance);

            controller.Dispose();
        }

        [Theory]
        [InlineData("CV1 2WT", null, false)]
        [InlineData("TestTown", null, true)]
        [InlineData("TownWithCords", "1.23|3.45", false)]
        public async Task AjaxChangedReturnAutoLocationSuggestions(string townOrPostcode, string coordinates, bool expectAutoLocationSuggest)
        {
            // arrange
            var controller = BuildCourseController(MediaTypeNames.Text.Html);
            var paramValues = new ParamValues
            {
                Town = townOrPostcode,
                Coordinates = coordinates,
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

            A.CallTo(() => FakeLocationsService.GetSuggestedLocationsAsync(A<string>.Ignored)).Returns(GetTestSuggestedLocations());
            A.CallTo(() => FakeFindACoursesService.GetFilteredData(A<CourseSearchFilters>.Ignored, A<CourseSearchOrderBy>.Ignored, A<int>.Ignored)).Returns(returnedCourseData);
            A.CallTo(() => FakeViewHelper.RenderViewAsync(A<CourseController>.Ignored, A<string>.Ignored, A<BodyViewModel>.Ignored, A<bool>.Ignored)).Returns("<p>some markup</p>");

            // act
            var result = await controller.AjaxChanged(appdata).ConfigureAwait(false);

            // assert
            if (expectAutoLocationSuggest)
            {
                A.CallTo(() => FakeLocationsService.GetSuggestedLocationsAsync(A<string>.Ignored)).MustHaveHappenedOnceExactly();
                result.AutoSuggestedTown.Should().Be("LN1 (LAN1)");
                result.UsingAutoSuggestedLocation.Should().BeTrue();
                result.DidYouMeanLocations.Count.Should().Be(1);
            }
            else
            {
                A.CallTo(() => FakeLocationsService.GetSuggestedLocationsAsync(A<string>.Ignored)).MustNotHaveHappened();
                result.UsingAutoSuggestedLocation.Should().BeFalse();
            }

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
            Assert.Equal(true, result.ShowDistanceSelector);

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
            await act.Should().ThrowAsync<ArgumentNullException>();
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
            var result = await controller.FilterResults(bodyViewModel, string.Empty).ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);
            Assert.Null(model.Results);

            controller.Dispose();
        }

        [Fact]
        public async Task FilterResultsSetsLocationFieldsWhenPassedIn()
        {
            // arrange
            var controller = BuildCourseController("*/*");

            var bodyViewModel = new BodyViewModel
            {
                CurrentSearchTerm = "Maths",
                SideBar = new SideBarViewModel(),
                IsTest = true,
            };

            // act
            var result = await controller.FilterResults(bodyViewModel, "TestLocation (Test Area)|-123.45|67.89").ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);
            model.SideBar.TownOrPostcode.Should().Be("TestLocation (Test Area)");
            model.SideBar.Coordinates.Should().Be("-123.45|67.89");
            controller.Dispose();
        }

        [Theory]
        [InlineData("Next 3 months", 0, 3, StartDate.SelectDateFrom)]
        [InlineData("In 3 to 6 months", 3, 6, StartDate.SelectDateFrom)]
        [InlineData("More than 6 months", 6, -1, StartDate.SelectDateFrom)]
        public async Task FilterResultsSetsStartDateValuesWhenPassedIn(string startDateValue, int from, int to, StartDate start)
        {
            // arrange
            var controller = BuildCourseController("*/*");

            var bodyViewModel = new BodyViewModel
            {
                CurrentSearchTerm = "Maths",
                SideBar = new SideBarViewModel()
                {
                    StartDateValue = startDateValue,
                },
                IsTest = true,
            };

            // act
            var result = await controller.FilterResults(bodyViewModel, "TestLocation (Test Area)|-123.45|67.89").ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);
            Assert.Equal(model.CourseSearchFilters.StartDate, start);
            Assert.Equal(model.CourseSearchFilters.StartDateFrom, DateTime.Today.AddMonths(from));
            Assert.Equal(model.CourseSearchFilters.StartDateTo, to == -1 ? DateTime.MinValue : DateTime.Today.AddMonths(to));
            controller.Dispose();
        }

        [Fact]
        public async Task FilterResultsSetsCampaignCode()
        {
            // arrange
            var controller = BuildCourseController("*/*");

            var bodyViewModel = new BodyViewModel
            {
                CurrentSearchTerm = "Maths",
                FreeCourseSearch = true,
                SideBar = new SideBarViewModel(),
                IsTest = true,
            };

            // act
            var result = await controller.FilterResults(bodyViewModel, "TestLocation (Test Area)|-123.45|67.89").ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);
            Assert.Equal(model.CourseSearchFilters.CampaignCode, CourseController.FreeSearchCampaignCode);
            controller.Dispose();
        }

        [Fact]
        public async Task FilterResultsIgnoreCampaignCodeWhenAlreadySet()
        {
            // arrange
            var controller = BuildCourseController("*/*");

            var bodyViewModel = new BodyViewModel
            {
                CurrentSearchTerm = "Maths",
                FreeCourseSearch = true,
                SideBar = new SideBarViewModel(),
                CourseSearchFilters = new CourseSearchFilters()
                {
                    CampaignCode = "test",
                },
                IsTest = true,
            };

            // act
            var result = await controller.FilterResults(bodyViewModel, "TestLocation (Test Area)|-123.45|67.89").ConfigureAwait(false);

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<BodyViewModel>(viewResult.ViewData.Model);
            Assert.NotEqual(model.CourseSearchFilters.CampaignCode, CourseController.FreeSearchCampaignCode);
            controller.Dispose();
        }

        [Fact]
        public async Task FilterResultsReturnsNotFoundForNullModel()
        {
            // arrange
            var controller = BuildCourseController("*/*");

            // act
            var actual = await controller.FilterResults(null, string.Empty);

            // assert
            actual.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.NotFound));

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

        [Fact]
        public async Task CourseControllerSearchFreeCourseThrowsException()
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
            await act.Should().ThrowAsync<ArgumentNullException>();

            controller.Dispose();
        }

        private IEnumerable<SearchLocationIndex> GetTestSuggestedLocations()
        {
            yield return new SearchLocationIndex()
            {
                LocationId = "1",
                LocationName = "LN1",
                LocalAuthorityName = "LAN1",
                LocationAuthorityDistrict = "LAD1",
                Longitude = 1.23,
                Latitude = 4.56,
            };

            yield return new SearchLocationIndex()
            {
                LocationId = "2",
                LocationName = "LN2",
                LocalAuthorityName = "LAN2",
                LocationAuthorityDistrict = "LAD2",
                Longitude = 21.23,
                Latitude = 24.56,
            };
        }
    }
}
