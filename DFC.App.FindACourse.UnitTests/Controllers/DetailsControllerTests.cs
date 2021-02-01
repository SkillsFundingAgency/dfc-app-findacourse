using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Services;
using DFC.CompositeInterfaceModels.FindACourseClient;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    public class DetailsControllerTests : BaseController
    {
        [Theory]
        [MemberData(nameof(HtmlMediaTypes))]
        public async Task DetailsControllerFilterResultReturnsSuccess(string mediaTypeName)
        {
            var courseService = A.Fake<IFindACourseService>();
            var logger = A.Fake<ILogger<DetailsController>>();
            var controller = BuildDetailsController(mediaTypeName);
            const string courseId = "c0a5dfeb-f2a6-4000-8272-ec1fa78df265";
            const string runId = "6707d15a-5a19-4c18-9cc8-570573bb5d67";

            var returnedCourseData = new CourseDetails
            {
                Title = "Maths in a unit test",
                Description = "This is a maths in a top class description",
                EntryRequirements = "Bring yourself and a brain",
            };

            A.CallTo(() => courseService.GetCourseDetails(courseId, runId)).Returns(returnedCourseData);

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
    }
}
