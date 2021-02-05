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

        private const string courseId = "c0a5dfeb-f2a6-4000-8272-ec1fa78df265";
        private const string runId = "6707d15a-5a19-4c18-9cc8-570573bb5d67";

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task FilterResultReturnsSuccess(string mediaTypeName)
        {
            var controller = BuildDetailsController(mediaTypeName);

            var returnedCourseData = new CourseDetails
            {
                Title = "Maths in a unit test",
                Description = "This is a maths in a top class description",
                EntryRequirements = "Bring yourself and a brain",
            };

            A.CallTo(() => FakeFindACoursesService.GetCourseDetails(courseId, runId)).Returns(returnedCourseData);

            var paramValues = new ParamValues
            {
                Page = 1,
                D = 1,
                OrderByValue = "StartDate",
            };
            var result = await controller.Details(courseId, runId, null, "Maths", paramValues).ConfigureAwait(false);

            var viewResult = Assert.IsType<ViewResult>(result);

            controller.Dispose();
        }

        [Fact]
        public async Task GetCourseDetailsReturnsFailedOnServiceErrors()
        {
            //Set Up
            A.CallTo(() => FakeFindACoursesService.GetCourseDetails(A<string>.Ignored, A<string>.Ignored)).Throws(new SystemException());

            var controller = BuildDetailsController("*/*");
            var paramValues = new ParamValues();

            //Act
            var result = await controller.Details(courseId, runId, null, "testSearchTerm", paramValues).ConfigureAwait(false);

            //Asserts
            var resultStatus = result as StatusCodeResult;
            resultStatus.StatusCode.Should().Be((int)HttpStatusCode.FailedDependency);
            A.CallTo(() => FakeLogService.LogError(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            controller.Dispose();
        }

        [Fact]
        public void GetCourseDEtailsThrowsExceptionForNullPramVaules()
        {
            // arrange
            var controller = BuildDetailsController("*/*");

            // act
            Func<Task> act = async () => await controller.Details(courseId, runId, null, "testSearchTerm", null).ConfigureAwait(false);

            // assert
            act.Should().Throw<ArgumentNullException>();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task GetTlevelSuccess(string mediaTypeName)
        {
            //Set Up
            A.CallTo(() => FakeFindACoursesService.GetTLevelDetails(A<string>.Ignored, A<string>.Ignored)).Returns(GetTestTLevel());

            var controller = BuildDetailsController(mediaTypeName);

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

            controller.Dispose();
        }

        [Fact]
        public async Task GetTlevelReturnsFailedOnServiceErrors()
        {
            //Set Up
            A.CallTo(() => FakeFindACoursesService.GetTLevelDetails(A<string>.Ignored, A<string>.Ignored)).Throws(new SystemException());

            var controller = BuildDetailsController("*/*");

            var paramValues = new ParamValues();

            //Act
            var result = await controller.TLevelDetails(TLevelId, TLevelLocationId, "testSearchTerm", paramValues).ConfigureAwait(false);

            //Asserts
            var resultStatus = result as StatusCodeResult;
            resultStatus.StatusCode.Should().Be((int) HttpStatusCode.FailedDependency);
            A.CallTo(() => FakeLogService.LogError(A<string>.Ignored)).MustHaveHappenedOnceExactly();

            controller.Dispose();
        }

        [Fact]
        public void GetTLevelThrowsExceptionForNullPramVaules()
        {
            // arrange
            var controller = BuildDetailsController("*/*");

            // act
            Func<Task> act = async () => await controller.TLevelDetails(TLevelId, TLevelLocationId, "testSearchTerm", null).ConfigureAwait(false);

            // assert
            act.Should().Throw<ArgumentNullException>();

            controller.Dispose();
        }

        private TLevelDetails GetTestTLevel()
        {
            return new TLevelDetails
            {
                TLevelId = Guid.Parse(TLevelId),
                Venues = new List<Venue>(),
            };
        }
    }
}
