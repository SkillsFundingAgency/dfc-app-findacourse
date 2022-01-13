using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.ViewModels;
using DFC.CompositeInterfaceModels.FindACourseClient;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    public class DetailsControllerTests : BaseController
    {
        private const string TLevelId = "7e52ca2c-783d-4596-983c-e81a1b549e4a";
        private const string TLevelLocationId = "bbaa3712-7f6a-4f28-a60c-50d449f7d483";

        private const string CourseId = "c0a5dfeb-f2a6-4000-8272-ec1fa78df265";
        private const string RunId = "6707d15a-5a19-4c18-9cc8-570573bb5d67";

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task FilterResultReturnsSuccessWithPostcodeAndRegions(string mediaTypeName)
        {
            using var controller = BuildDetailsController(mediaTypeName);

            var returnedCourseData = new CourseDetails
            {
                Title = "Maths in a unit test",
                Description = "This is a maths in a top class description",
                EntryRequirements = "Bring yourself and a brain",
                SubRegions = new List<SubRegion>
                {
                    new SubRegion
                    {
                        SubRegionId = Guid.NewGuid().ToString(),
                        Name = "sub-region-1",
                        ParentRegion = new ParentRegion
                        {
                            RegionId = Guid.NewGuid().ToString(),
                            Name = "region-1",
                        },
                    },
                    new SubRegion
                    {
                        SubRegionId = Guid.NewGuid().ToString(),
                        Name = "sub-region-2",
                        ParentRegion = new ParentRegion
                        {
                            RegionId = Guid.NewGuid().ToString(),
                            Name = "region-2",
                        },
                    },
                },
            };

            A.CallTo(() => FakeFindACoursesService.GetCourseDetails(CourseId, RunId)).Returns(returnedCourseData);

            var paramValues = new ParamValues
            {
                Page = 1,
                D = 1,
                OrderByValue = "StartDate",
                Town = "CV1 2WT",
            };
            var result = await controller.Details(CourseId, null, RunId, "Maths", paramValues).ConfigureAwait(false);

            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task FilterResultReturnsSuccessWithoutPostcodeOrRegions(string mediaTypeName)
        {
            using var controller = BuildDetailsController(mediaTypeName);

            var returnedCourseData = new CourseDetails
            {
                Title = "Maths in a unit test",
                Description = "This is a maths in a top class description",
                EntryRequirements = "Bring yourself and a brain",
            };

            A.CallTo(() => FakeFindACoursesService.GetCourseDetails(CourseId, RunId)).Returns(returnedCourseData);

            var paramValues = new ParamValues
            {
                Page = 1,
                D = 1,
                OrderByValue = "StartDate",
            };
            var result = await controller.Details(CourseId, null, RunId, "Maths", paramValues).ConfigureAwait(false);

            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData("", HttpStatusCode.ServiceUnavailable)]
        [InlineData("Not Found 404", HttpStatusCode.NotFound)]
        public async Task GetCourseDetailsReturnsFailedOnServiceErrors(string errorMesage, HttpStatusCode expectedHttpStatusCode)
        {
            //Set Up
            A.CallTo(() => FakeFindACoursesService.GetCourseDetails(A<string>.Ignored, A<string>.Ignored)).Throws(new SystemException(errorMesage));

            using var controller = BuildDetailsController("*/*");
            var paramValues = new ParamValues();

            //Act
            var result = await controller.Details(CourseId, RunId, null, "testSearchTerm", paramValues).ConfigureAwait(false);

            //Asserts
            var resultStatus = result as StatusCodeResult;
            resultStatus.StatusCode.Should().Be((int)expectedHttpStatusCode);
            A.CallTo(() => FakeLogService.LogError(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetCourseDetailsReturnsNotFound()
        {
            //Set Up
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.NotFound;
            const CourseDetails nullCourseSearchResult = null;
            A.CallTo(() => FakeFindACoursesService.GetCourseDetails(A<string>.Ignored, A<string>.Ignored)).Returns(nullCourseSearchResult);

            using var controller = BuildDetailsController("*/*");
            var paramValues = new ParamValues();

            //Act
            var result = await controller.Details(CourseId, RunId, null, "testSearchTerm", paramValues).ConfigureAwait(false);

            //Asserts
            var resultStatus = result as StatusCodeResult;
            resultStatus.StatusCode.Should().Be((int)expectedHttpStatusCode);
            A.CallTo(() => FakeLogService.LogWarning(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetCourseDetailsReturnsBadRequestorNullParamValues()
        {
            // arrange
            using var controller = BuildDetailsController("*/*");

            // act
            var actual = await controller.Details(CourseId, RunId, null, "testSearchTerm", null);

            // assert
            actual.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.BadRequest));
        }

        [Fact]
        public async Task GetCourseDetailsReturnsNotFoundForNullCourseId()
        {
            // arrange
            using var controller = BuildDetailsController("*/*");
            var paramValues = new ParamValues
            {
                Page = 1,
                D = 1,
                OrderByValue = "StartDate",
            };

            // act
            var actual = await controller.Details(null, RunId, null, "testSearchTerm", paramValues);

            // assert
            actual.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.NotFound));
        }

        [Fact]
        public async Task GetCourseDetailsReturnsNotFoundForNullRunId()
        {
            // arrange
            using var controller = BuildDetailsController("*/*");
            var paramValues = new ParamValues
            {
                Page = 1,
                D = 1,
                OrderByValue = "StartDate",
            };

            // act
            var actual = await controller.Details(CourseId, null, null, "testSearchTerm", paramValues);

            // assert
            actual.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.NotFound));
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task GetTlevelSuccess(string mediaTypeName)
        {
            //Set Up
            A.CallTo(() => FakeFindACoursesService.GetTLevelDetails(A<string>.Ignored, A<string>.Ignored)).Returns(GetTestTLevel());

            using var controller = BuildDetailsController(mediaTypeName);

            var paramValues = new ParamValues
            {
                Page = 1,
                D = 1,
                OrderByValue = "StartDate",
            };

            //Act
            var result = await controller.TLevelDetails(TLevelId, TLevelLocationId, "testSearchTerm", paramValues).ConfigureAwait(false);

            //Asserts
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<TLevelDetailsViewModel>(viewResult.ViewData.Model);
            model.TlevelDetails.TLevelId.Should().Be(Guid.Parse(TLevelId));
        }

        [Fact]
        public async Task GetTlevelReturnsNotFound()
        {
            //Set Up
            const HttpStatusCode expectedHttpStatusCode = HttpStatusCode.NotFound;
            const TLevelDetails nullTLevelDetails = null;
            A.CallTo(() => FakeFindACoursesService.GetTLevelDetails(A<string>.Ignored, A<string>.Ignored)).Returns(nullTLevelDetails);

            using var controller = BuildDetailsController("*/*");
            var paramValues = new ParamValues();

            //Act
            var result = await controller.TLevelDetails(CourseId, TLevelLocationId, "testSearchTerm", paramValues).ConfigureAwait(false);

            //Asserts
            var resultStatus = result as StatusCodeResult;
            resultStatus.StatusCode.Should().Be((int)expectedHttpStatusCode);
            A.CallTo(() => FakeLogService.LogWarning(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetTLevelReturnsBadRequestForNullParamValues()
        {
            // arrange
            using var controller = BuildDetailsController("*/*");

            // act
            var actual = await controller.TLevelDetails(TLevelId, TLevelLocationId, "testSearchTerm", null);

            // assert
            actual.Should().BeEquivalentTo(new StatusCodeResult((int)HttpStatusCode.BadRequest));
        }

        [Theory]
        [InlineData("", HttpStatusCode.ServiceUnavailable)]
        [InlineData("Not Found 404", HttpStatusCode.NotFound)]
        public async Task GetTLevelDetailsReturnsFailedOnServiceErrors(string errorMesage, HttpStatusCode expectedHttpStatusCode)
        {
            //Set Up
            A.CallTo(() => FakeFindACoursesService.GetTLevelDetails(A<string>.Ignored, A<string>.Ignored)).Throws(new SystemException(errorMesage));

            using var controller = BuildDetailsController("*/*");
            var paramValues = new ParamValues();

            //Act
            var result = await controller.TLevelDetails(CourseId, TLevelLocationId, "testSearchTerm", paramValues).ConfigureAwait(false);

            //Asserts
            var resultStatus = result as StatusCodeResult;
            resultStatus.StatusCode.Should().Be((int)expectedHttpStatusCode);
            A.CallTo(() => FakeLogService.LogError(A<string>.Ignored)).MustHaveHappenedOnceExactly();
        }

        private static TLevelDetails GetTestTLevel()
        {
            return new TLevelDetails
            {
                TLevelId = Guid.Parse(TLevelId),
                Venues = new List<Venue>(),
            };
        }
    }
}
