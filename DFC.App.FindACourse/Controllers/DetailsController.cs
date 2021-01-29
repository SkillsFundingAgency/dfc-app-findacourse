using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Extensions;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.CompositeInterfaceModels.FindACourseClient;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
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
        public async Task<IActionResult> Details(string courseId, string runId, string currentSearchTerm, ParamValues paramValues)
        {
            logService.LogInformation($"{nameof(this.Details)} has been called");
            var model = new DetailsViewModel();
            if (paramValues.SearchTerm == null && currentSearchTerm != null)
            {
                paramValues.SearchTerm = currentSearchTerm;
            }

            var isPostcode = !string.IsNullOrEmpty(paramValues.Town) ? (bool?)paramValues.Town.IsPostcode() : null;
            paramValues.D = isPostcode.HasValue && isPostcode.Value ? 1 : 0;

            model.SearchTerm = $"{nameof(paramValues.SearchTerm)}={paramValues.SearchTerm}&" +
                               $"{nameof(paramValues.Town)}={paramValues.Town}&" +
                               $"{nameof(paramValues.CourseType)}={paramValues.CourseType}&" +
                               $"{nameof(paramValues.CourseHours)}={paramValues.CourseHours}&" +
                               $"{nameof(paramValues.CourseStudyTime)}={paramValues.CourseStudyTime}&" +
                               $"{nameof(paramValues.StartDate)}={paramValues.StartDate}&" +
                               $"{nameof(paramValues.Distance)}={paramValues.Distance}&" +
                               $"{nameof(paramValues.FilterA)}={paramValues.FilterA}&" +
                               $"{nameof(paramValues.Page)}={paramValues.Page}&" +
                               $"{nameof(paramValues.D)}={paramValues.D}&" +
                               $"{nameof(paramValues.OrderByValue)}={paramValues.OrderByValue}";

            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(runId))
            {
                throw new ArgumentNullException("Course Id and/or runId does not have a value");
            }

            try
            {
                model.SpeakToAnAdviser = await staticContentDocumentService.GetByIdAsync(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false);
                model.CourseDetails = await findACourseService.GetCourseDetails(courseId, runId).ConfigureAwait(false);
                model.CourseRegions = model.CourseDetails.SubRegions != null ? TransformSubRegionsToRegions(model.CourseDetails.SubRegions) : null;
            }
            catch (Exception ex)
            {
                logService.LogError($"Get course details caused an error: {ex}. " +
                    $"The values passed were: course id: {courseId} and run id: {runId}");
            }

            return View(model);
        }

        private static IList<CourseRegion> TransformSubRegionsToRegions(IList<SubRegion> subRegions)
        {
            var result = (from a in subRegions select a.ParentRegion.Name)
                          .OrderBy(o => o).Distinct()
                          .Select(s => new CourseRegion
                          {
                              Name = s,
                              SubRegions = (from sr in subRegions where sr.ParentRegion.Name == s select sr.Name).OrderBy(o => o).ToList(),
                          })
                          .ToList();

            return result;
        }
    }
}