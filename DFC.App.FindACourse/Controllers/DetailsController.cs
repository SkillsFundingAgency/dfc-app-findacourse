using AutoMapper;
using DFC.App.FindACourse.Data.Models;
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
        private readonly IMapper mapper;

        public DetailsController(
            ILogService logService,
            IFindACourseService findACourseService,
            IDocumentService<StaticContentItemModel> staticContentDocumentService,
            CmsApiClientOptions cmsApiClientOptions,
            IMapper mapper)
        {
            this.logService = logService;
            this.findACourseService = findACourseService;
            this.staticContentDocumentService = staticContentDocumentService;
            this.cmsApiClientOptions = cmsApiClientOptions;
            this.mapper = mapper;
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
                model.CourseDetails = await findACourseService.GetCourseDetails(courseId, runId).ConfigureAwait(false);
                model.CourseRegions = model.CourseDetails.SubRegions != null ? TransformSubRegionsToRegions(model.CourseDetails.SubRegions) : null;
                model.DetailsRightBarViewModel.Provider = mapper.Map<ProviderViewModel>(model.CourseDetails.ProviderDetails);
                model.DetailsRightBarViewModel.SpeakToAnAdviser = await staticContentDocumentService.GetByIdAsync(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logService.LogError($"Get course details caused an error: {ex}. " +
                    $"The values passed were: course id: {courseId} and run id: {runId}");
            }

            return View(model);
        }

        [HttpGet]
        [Route("find-a-course/search/tdetails/body")]
        [Route("find-a-course/tdetails/body")]
        public async Task<IActionResult> TLevelDetails(string tlevelId, string searchTerm, string currentSearchTerm, string town, string courseType,
                                             string courseHours, string courseStudyTime, string startDate, string distance, string filtera, int page, int d, string orderByValue)
        {
            logService.LogInformation($"{nameof(this.TLevelDetails)} has been called");
            var model = new TLevelDetailsViewModel();
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

            if (string.IsNullOrEmpty(tlevelId))
            {
                throw new ArgumentNullException("Course Id and/or runId does not have a value");
            }

            try
            {
                model.TlevelDetails = await findACourseService.GetTLevelDetails(tlevelId).ConfigureAwait(false);
                model.DetailsRightBarViewModel.Provider = mapper.Map<ProviderViewModel>(model.TlevelDetails.ProviderDetails);
                model.DetailsRightBarViewModel.SpeakToAnAdviser = await staticContentDocumentService.GetByIdAsync(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logService.LogError($"Get course details caused an error: {ex}. " +
                    $"The values passed were: course id: {tlevelId}");
            }

            return View("tlevelDetails", model);
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