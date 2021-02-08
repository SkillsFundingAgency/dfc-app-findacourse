﻿using AutoMapper;
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
using System.Net;
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
        [Route("find-a-course/search/course-details/body")]
        public async Task<IActionResult> Details(string courseId, string runId, string r, string currentSearchTerm, ParamValues paramValues)
        {
            logService.LogInformation($"{nameof(this.Details)} has been called");

            if (paramValues == null)
            {
                throw new ArgumentNullException(nameof(paramValues));
            }

            var model = new DetailsViewModel();

            runId ??= r;

            model.SearchTerm = FormatSearchParameters(paramValues, currentSearchTerm);

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
                logService.LogError($"Get course details caused an error: {ex}. The values passed were: course id: {courseId} and run id: {runId}");

                //Return an error code to cause the problem page to be displayed, previously this was returning OK with an empty model,
                //this causes errors in the view and then goes to the problem page
                return StatusCode((int)HttpStatusCode.FailedDependency);
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
                throw new ArgumentNullException(nameof(paramValues));
            }

            var model = new TLevelDetailsViewModel();

            if (paramValues.SearchTerm == null && currentSearchTerm != null)
            {
                paramValues.SearchTerm = currentSearchTerm;
            }

            model.SearchTerm = FormatSearchParameters(paramValues, currentSearchTerm);

            try
            {
                model.TlevelDetails = await findACourseService.GetTLevelDetails(tlevelId, tlevelLocationId).ConfigureAwait(false);
                model.DetailsRightBarViewModel.Provider = mapper.Map<ProviderViewModel>(model.TlevelDetails.ProviderDetails);
                model.DetailsRightBarViewModel.SpeakToAnAdviser = await staticContentDocumentService.GetByIdAsync(new Guid(cmsApiClientOptions.ContentIds)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logService.LogError($"Get tlevel details caused an error: {ex}. The values passed were: tlevel id: {tlevelId}");
                return StatusCode((int)HttpStatusCode.FailedDependency);
            }

            return View("tlevelDetails", model);
        }

        private static string FormatSearchParameters(ParamValues paramValues, string currentSearchTerm)
        {
            if (paramValues.SearchTerm == null && currentSearchTerm != null)
            {
                paramValues.SearchTerm = currentSearchTerm;
            }

            var isPostcode = !string.IsNullOrEmpty(paramValues.Town) ? (bool?)paramValues.Town.IsPostcode() : null;
            paramValues.D = isPostcode.HasValue && isPostcode.Value ? 1 : 0;

            var searchTerm = $"{nameof(paramValues.SearchTerm)}={paramValues.SearchTerm}&" +
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
    }
}