using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Controllers
{
    public class DetailsController : Controller
    {
        private readonly ILogService logService;
        private readonly IFindACourseService findACourseService;
        private readonly IDocumentService<StaticContentItemModel> staticContentDocumentService;
        private readonly CmsApiClientOptions cmsApiClientOptions;

        public DetailsController(
            ILogService logService,
            IFindACourseService findACourseService,
            IDocumentService<StaticContentItemModel> staticContentDocumentService,
            CmsApiClientOptions cmsApiClientOptions)
        {
            this.logService = logService;
            this.findACourseService = findACourseService;
            this.staticContentDocumentService = staticContentDocumentService;
            this.cmsApiClientOptions = cmsApiClientOptions;
        }

        [HttpGet]
        [Route("find-a-course/search/details/body")]
        [Route("find-a-course/details/body")]
        public async Task<IActionResult> Details(string courseId, string runId, string searchTerm, string currentSearchTerm, string town, string courseType,
                                                 string courseHours, string courseStudyTime, string startDate, string distance, string filtera, int page, int d, string orderByValue)
        {
            logService.LogInformation($"{nameof(this.Details)} has been called");
            var model = new DetailsViewModel();
            if (searchTerm == null && currentSearchTerm != null)
            {
                searchTerm = currentSearchTerm;
            }

            model.SearchTerm = $"{nameof(searchTerm)}={searchTerm}&" +
                               $"{nameof(town)}={town}&" +
                               $"{nameof(courseType)}={courseType}&" +
                               $"{nameof(courseHours)}={courseHours}&" +
                               $"{nameof(courseStudyTime)}={courseStudyTime}&" +
                               $"{nameof(startDate)}={startDate}&" +
                               $"{nameof(distance)}={distance}&" +
                               $"{nameof(filtera)}={filtera}&" +
                               $"{nameof(page)}={page}&" +
                               $"{nameof(d)}={d}&" +
                               $"{nameof(orderByValue)}={orderByValue}";

            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(runId))
            {
                throw new ArgumentNullException("Course Id and/or runId does not have a value");
            }

            try
            {
                model.SpeakToAnAdviser = await staticContentDocumentService.GetByIdAsync(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false);
                model.CourseDetails = await findACourseService.GetCourseDetails(courseId, runId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logService.LogError($"Get course details caused an error: {ex}. " +
                    $"The values passed were: course id: {courseId} and run id: {runId}");
            }

            return View(model);
        }
    }
}