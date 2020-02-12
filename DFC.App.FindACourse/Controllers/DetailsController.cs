using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace DFC.App.FindACourse.Controllers
{
    public class DetailsController : Controller
    {
        private readonly ILogger<CourseController> logger;
        private readonly IFindACourseService findACourseService;

        public DetailsController(ILogger<CourseController> logger, IFindACourseService findACourseService)
        {
            this.logger = logger;
            this.findACourseService = findACourseService;
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/details")]
        public IActionResult Details(string courseId, string runId)
        {
            this.logger.LogInformation($"{nameof(this.Details)} has been called");
            var model = new DetailsViewModel();

            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(runId))
            {
                throw new Exception("Course Id and/or runId does not have a value");
            }

            try
            {
               model.courseDetails = this.findACourseService.GetCourseDetails(courseId, runId).Result;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation($"Get course details caused an error: {ex}");
            }

            return View(model);
        }
    }
}