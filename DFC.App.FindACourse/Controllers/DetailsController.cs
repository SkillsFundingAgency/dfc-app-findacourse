using DFC.App.FindACourse.Cache;
using DFC.App.FindACourse.Data.Contracts;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public DetailsController(ILogService logService,
            IFindACourseService findACourseService,
            ISharedContentService sharedContentService,
            ICacheService cacheService
            )
        {
            this.logService = logService;
            this.findACourseService = findACourseService;
            this.sharedContentService = sharedContentService;
            this.cacheService = cacheService;
        }

        [HttpGet]
        [Route("find-a-course/search/details/body")]
        [Route("find-a-course/details/body")]
        public async Task<IActionResult> Details(string courseId, string runId, string searchTerm, string CurrentSearchTerm, string town, string courseType,
                                                      string courseHours, string courseStudyTime, string courseStartDate, string distance, string filtera, int page, int d)
        {
            this.logService.LogInformation($"{nameof(this.Details)} has been called");
            var model = new DetailsViewModel();
            if (searchTerm == null && CurrentSearchTerm != null)
            {
                searchTerm = CurrentSearchTerm;
            }
            var contentList = new List<string>() { "speaktoanadvisor" };
            model.SearchTerm = $"searchTerm={searchTerm}&town={town}&courseType={courseType}&courseHours={courseHours}&studyTime={courseStudyTime}&startDate={courseStartDate}&distance={distance}&filtera={filtera}&page={page}&d={d}";

            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(runId))
            {
                throw new ArgumentNullException("Course Id and/or runId does not have a value");
            }

            try
            {
                var sharedContent = this.cacheService.GetFromCache<List<StaticContentItemModel>>(SharedContent);
                if (sharedContent == null)
                {
                    sharedContent = this.cacheService.GetOrSet<List<StaticContentItemModel>>(SharedContent, await this.sharedContentService.GetByNamesAsync(contentList).ConfigureAwait(false));
                }

                model.SpeakToAnAdvisor = sharedContent.FirstOrDefault();
                model.CourseDetails = await this.findACourseService.GetCourseDetails(courseId, runId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logService.LogError($"Get course details caused an error: {ex}. " +
                    $"The values passed were: course id: {courseId} and run id: {runId}");
            }

            return View(model);
        }
    }
}