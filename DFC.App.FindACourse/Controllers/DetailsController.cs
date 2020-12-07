using DFC.App.FindACourse.Cache;
using DFC.App.FindACourse.Data.Contracts;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Controllers
{
    public class DetailsController : Controller
    {
        private const string SharedContent = "SHARED_CONTENT";
        private readonly ILogService logService;
        private readonly IFindACourseService findACourseService;
        private readonly ISharedContentService sharedContentService;
        private readonly ICacheService cacheService;
        private readonly CmsApiClientOptions cmsApiClientOptions;

        public DetailsController(
            ILogService logService,
            IFindACourseService findACourseService,
            ISharedContentService sharedContentService,
            ICacheService cacheService,
            CmsApiClientOptions cmsApiClientOptions)
        {
            this.logService = logService;
            this.findACourseService = findACourseService;
            this.sharedContentService = sharedContentService;
            this.cacheService = cacheService;
            this.cmsApiClientOptions = cmsApiClientOptions;
        }

        [HttpGet]
        [Route("find-a-course/search/details/body")]
        [Route("find-a-course/details/body")]
        public async Task<IActionResult> Details(string courseId, string runId, string searchTerm, string currentSearchTerm, string town, string courseType,
                                                      string courseHours, string courseStudyTime, string courseStartDate, string distance, string filtera, int page, int d)
        {
            logService.LogInformation($"{nameof(this.Details)} has been called");
            var model = new DetailsViewModel();
            if (searchTerm == null && currentSearchTerm != null)
            {
                searchTerm = currentSearchTerm;
            }

            model.SearchTerm = $"searchTerm={searchTerm}&town={town}&courseType={courseType}&courseHours={courseHours}&studyTime={courseStudyTime}&startDate={courseStartDate}&distance={distance}&filtera={filtera}&page={page}&d={d}";

            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(runId))
            {
                throw new ArgumentNullException("Course Id and/or runId does not have a value");
            }

            try
            {
                model.SpeakToAnAdviser = cacheService.GetOrSet<StaticContentItemModel>(SharedContent, await sharedContentService.GetById(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false));
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