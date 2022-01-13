using AutoMapper;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.Logger.AppInsights.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SubRegion = DFC.CompositeInterfaceModels.FindACourseClient.SubRegion;

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
        [Route("find-a-course/search/course-details/body")]
        public async Task<IActionResult> Details(string courseId, string runId, string r, string currentSearchTerm, ParamValues paramValues)
        {
            logService.LogInformation($"{nameof(this.Details)} has been called");

            if (paramValues == null)
            {
                logService.LogError($"paramValues is null for method: {nameof(Details)} on controller {nameof(DetailsController)}");
                return BadRequest();
            }

            var model = new DetailsViewModel();

            runId ??= r;

            model.SearchTerm = FormatSearchParameters(paramValues, currentSearchTerm);
            if (Request.Headers.TryGetValue(HeaderNames.Referer, out var refererValues))
            {
                model.BackLinkUrl = refererValues.FirstOrDefault(x => x.Contains("job-profiles"));
            }

            if (string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(runId))
            {
                logService.LogError($"Course Id ({courseId}) and/or runId ({runId}) does not have a value - returning NotFound");
                return NotFound();
            }

            try
            {
                model.CourseDetails = await findACourseService.GetCourseDetails(courseId, runId).ConfigureAwait(false);
                if (model.CourseDetails == null)
                {
                    logService.LogWarning($"Get course details retrieved no data. The values passed were: course id: {courseId} and run id: {runId}");
                    return NotFound();
                }

                model.CourseRegions = model.CourseDetails.SubRegions != null ? TransformSubRegionsToRegions(model.CourseDetails.SubRegions) : null;
                model.DetailsRightBarViewModel.Provider = mapper.Map<ProviderViewModel>(model.CourseDetails.ProviderDetails);
                model.DetailsRightBarViewModel.SpeakToAnAdviser = await staticContentDocumentService.GetByIdAsync(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false);
                model.CourseDetails.CourseWebpageLink = CompareProviderLinkWithCourseLink(model?.CourseDetails?.CourseWebpageLink, model.CourseDetails?.ProviderDetails?.Website);
                model.CourseDetails.AdvancedLearnerLoansOffered = paramValues.CampaignCode == "LEVEL3_FREE";
            }
            catch (Exception ex)
            {
                logService.LogError($"Get course details caused an error: {ex}. The values passed were: course id: {courseId} and run id: {runId}");
                return DetailsErrorReturnStatus(ex);
            }

            return View(model);
        }

        [HttpGet]
        [Route("find-a-course/search/tdetails/body")]
        [Route("find-a-course/tdetails/body")]
        public async Task<IActionResult> TLevelDetails(string tlevelId, string tlevelLocationId, string currentSearchTerm, ParamValues paramValues)
        {
            logService.LogInformation($"{nameof(this.TLevelDetails)} has been called");

            if (paramValues == null)
            {
                logService.LogError($"paramValues is null for method: {nameof(TLevelDetails)} on controller {nameof(DetailsController)}");
                return BadRequest();
            }

            var model = new TLevelDetailsViewModel();

            if (paramValues.SearchTerm == null && currentSearchTerm != null)
            {
                paramValues.SearchTerm = currentSearchTerm;
            }

            try
            {
                model.SearchTerm = FormatSearchParameters(paramValues, currentSearchTerm);
                model.TlevelDetails = await findACourseService.GetTLevelDetails(tlevelId, tlevelLocationId).ConfigureAwait(false);
                if (model.TlevelDetails == null)
                {
                    logService.LogWarning($"Get TLevel details retrieved no data. The values passed were: tlevel id: {tlevelId} and run id: {tlevelLocationId}");
                    return NotFound();
                }

                model.DetailsRightBarViewModel.Provider = mapper.Map<ProviderViewModel>(model.TlevelDetails.ProviderDetails);
                model.DetailsRightBarViewModel.SpeakToAnAdviser = await staticContentDocumentService.GetByIdAsync(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logService.LogError($"Get TLevel details caused an error: {ex}. The values passed were: tlevel id: {tlevelId} and location id: {tlevelLocationId}");
                return DetailsErrorReturnStatus(ex);
            }

            return View("tlevelDetails", model);
        }

        private static string FormatSearchParameters(ParamValues paramValues, string currentSearchTerm)
        {
            if (paramValues.SearchTerm == null && currentSearchTerm != null)
            {
                paramValues.SearchTerm = currentSearchTerm;
            }

            var searchTerm = $"{nameof(paramValues.SearchTerm)}={paramValues.SearchTerm}&" +
                             $"{nameof(paramValues.Town)}={WebUtility.HtmlEncode(paramValues.Town)}&" +
                             $"{nameof(paramValues.CourseType)}={paramValues.CourseType}&" +
                             $"{nameof(paramValues.CourseHours)}={paramValues.CourseHours}&" +
                             $"{nameof(paramValues.CourseStudyTime)}={paramValues.CourseStudyTime}&" +
                             $"{nameof(paramValues.StartDate)}={paramValues.StartDate}&" +
                             $"{nameof(paramValues.Distance)}={paramValues.Distance}&" +
                             $"{nameof(paramValues.FilterA)}={paramValues.FilterA}&" +
                             $"{nameof(paramValues.Page)}={paramValues.Page}&" +
                             $"{nameof(paramValues.D)}={paramValues.D}&" +
                             $"{nameof(paramValues.OrderByValue)}={paramValues.OrderByValue}&" +
                             $"{nameof(paramValues.Coordinates)}={WebUtility.HtmlEncode(paramValues.Coordinates)}&" +
                             $"{nameof(paramValues.CampaignCode)}={paramValues.CampaignCode}";

            return searchTerm;
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

        private static string CompareProviderLinkWithCourseLink(string courseLink, string providerLink)
        {
            return string.IsNullOrEmpty(courseLink) ? null : courseLink.Equals(providerLink) ? null : courseLink;
        }

        private StatusCodeResult DetailsErrorReturnStatus(Exception ex)
        {
            //Return an error code to cause the problem page to be displayed, previously this was returning OK with an empty model,
            //this causes errors in the view and then goes to the problem page
            if (ex.Message.Contains("404"))
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }
            else
            {
                return StatusCode((int)HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}