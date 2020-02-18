using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Controllers
{
    public class DetailsController : Controller
    {
        private readonly ILogger<DetailsController> logger;
        private readonly IFindACourseService findACourseService;

        public DetailsController(ILogger<DetailsController> logger, IFindACourseService findACourseService)
        {
            this.logger = logger;
            this.findACourseService = findACourseService;
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/details")]
        public async Task<IActionResult> Details(string courseId, string runId, string searchTerm)
        {
            this.logger.LogInformation($"{nameof(this.Details)} has been called");
            var model = new DetailsViewModel();
            model.SearchTerm = searchTerm;

            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(runId))
            {
                throw new ArgumentNullException("Course Id and/or runId does not have a value");
            }

            try
            {
                //WARNING - DO NOT LET THIS THROUGH - fAC CLIENT DOWN SO WORK AROUND
                courseId = "4b04ac96-4d20-4021-b986-f6c4c2fffbc8";
                runId = "9202aece-e1f0-4347-b6a3-5c63ada706f1";
                /////END WARNING///////////////////

               model.CourseDetails = await this.findACourseService.GetCourseDetails(courseId, runId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Get course details caused an error: {ex}. " +
                    $"The values passed were: course id: {courseId} and run id: {runId}");
            }

            return View(model);
        }
    }
}