﻿using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Data.Helpers;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Extensions;
using DFC.App.FindACourse.Helpers;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.CompositeInterfaceModels.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using GdsCheckboxList.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Fac = DFC.FindACourseClient;

namespace DFC.App.FindACourse.Controllers
{
    public class CourseController : Controller
    {
        public const string FreeSearchCampaignCode = "LEVEL3_FREE";
        private readonly ILogService logService;
        private readonly IFindACourseService findACourseService;
        private readonly IViewHelper viewHelper;
        private readonly ILocationService locationService;
        private readonly Fac.CourseSearchClientSettings courseSearchClientSettings;
        private readonly CourseSearchSettings courseSearchSettings;

        public CourseController(ILogService logService, IFindACourseService findACourseService, IViewHelper viewHelper, ILocationService locationService, Fac.CourseSearchClientSettings courseSearchClientSettings, CourseSearchSettings courseSearchSettings)
        {
            this.logService = logService;
            this.findACourseService = findACourseService;
            this.viewHelper = viewHelper;
            this.locationService = locationService;
            this.courseSearchClientSettings = courseSearchClientSettings;
            this.courseSearchSettings = courseSearchSettings;
        }

        [HttpGet]
        [Route("search/")]
        public IActionResult Index()
        {
            logService.LogInformation($"{nameof(Index)} has been called");
            var viewModel = new IndexViewModel
            {
                Documents = new List<IndexDocumentViewModel> { new IndexDocumentViewModel { CanonicalName = "Index" } },
            };

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        [Route("search/document")]
        public IActionResult Document()
        {
            logService.LogInformation($"{nameof(this.Document)} has been called");

            var model = new DocumentViewModel
            {
                Title = "Find a Course",
                BreadcrumbTitle = "Breadcrumb 1",
                Keywords = "fac",
                CanonicalName = "https://nationalcareers.service.gov.uk/find-a-course",
                Content = new HtmlString("Document Content"),
                IncludeInSitemap = true,
            };

            logService.LogInformation($"{nameof(this.Document)} generated the model and ready to pass to the view");

            return View(model);
        }

        [HttpGet]
        [Route("course/search/{articleName}/head")]
        [Route("course/search/search/head")]
        [Route("course/search/head")]
        public IActionResult Head(string articleName)
        {
            logService.LogInformation($"{nameof(this.Head)} has been called");

            string title = "Results";

            switch (articleName)
            {
                case "course":
                    title = "Results";
                    break;
                case "details":
                    title = "Details";
                    break;
                default:
                    break;
            }

            var model = new HeadViewModel { Title = $"{title} | Find a course | National Careers Service", Description = "FAC", Keywords = "fac", CanonicalUrl = "https://nationalcareers.service.gov.uk/find-a-course", IsHidden = string.Compare(articleName, "searchFreeCourse", StringComparison.CurrentCultureIgnoreCase) == 0 };

            logService.LogInformation($"{nameof(this.Head)} generated the model and ready to pass to the view");

            return View(model);
        }

        [HttpGet]
        [ResponseCache(Duration = 43200)]
        public IActionResult Breadcrumb()
        {
            logService.LogInformation($"{nameof(this.Breadcrumb)} has been called");

            var model = new BreadcrumbViewModel
            {
                Paths = new List<BreadcrumbPathViewModel>
                {
                    new BreadcrumbPathViewModel { Route = "/", Title = "Home", AddHyperlink = true },
                    new BreadcrumbPathViewModel { Route = "/find-a-course", Title = "Find a Course", AddHyperlink = true },
                },
            };

            logService.LogInformation($"{nameof(this.Breadcrumb)} generated the model and ready to pass to the view");
            return View(model);
        }

        [HttpGet]
        [Route("find-a-course/search/find-courses-to-get-a-job/body")]
        [Route("find-a-course/search/find-courses-to-get-a-job")]
        public IActionResult FindCoursedToGetAJob()
        {
            logService.LogInformation($"{nameof(FindCoursedToGetAJob)} has been called");

            var model = new BodyViewModel { Content = new HtmlString("Find a course: Body element") };
            model.CourseSearchSettings = courseSearchSettings;
            model.SideBar = GetSideBarViewModel();
            return View("find-courses-to-get-a-job", model);
        }

        [HttpGet]
        [Route("find-a-course/search/help-with-choices/body")]
        [Route("find-a-course/search/help-with-choices")]
        public IActionResult HelpWithChoices()
        {
            logService.LogInformation($"{nameof(HelpWithChoices)} has been called");

            var model = new BodyViewModel { Content = new HtmlString("Find a course: Body element") };
            model.CourseSearchSettings = courseSearchSettings;
            model.SideBar = GetSideBarViewModel();
            return View("help-with-choices", model);
        }

        [HttpGet]
        [Route("find-a-course/search/body")]
        [Route("find-a-course/search/search/body")]
        [ResponseCache(Duration = 43200)]
        public IActionResult Body()
        {
            logService.LogInformation($"{nameof(this.Body)} has been called");

            var model = new BodyViewModel { Content = new HtmlString("Find a course: Body element") };
            model.SideBar = GetSideBarViewModel();
            model.SideBar.OrderByOptions = ListFilters.GetOrderByOptions();
            model.CourseSearchSettings = courseSearchSettings;

            logService.LogInformation($"{nameof(this.Body)} generated the model and ready to pass to the view");

            return View("Home", model);
        }

        [HttpGet]
        [Route("find-a-course/search/{articleName}/bodyfooter")]
        [Route("find-a-course/search/bodyfooter")]
        [ResponseCache(Duration = 43200)]
        public IActionResult BodyFooter(string articleName)
        {
            logService.LogInformation($"{nameof(this.BodyFooter)} has been called");

            return NoContent();
        }

        [HttpGet]
        [Route("api/get/find-a-course/search/{appData}/ajax")]
        public async Task<AjaxModel> AjaxChanged(string appData)
        {
            logService.LogInformation($"{nameof(AjaxChanged)} has been called");
            if (string.IsNullOrWhiteSpace(appData))
            {
                throw new ArgumentNullException(nameof(appData));
            }

            var paramValues = System.Text.Json.JsonSerializer.Deserialize<ParamValues>(appData);

            var model = new BodyViewModel
            {
                CurrentSearchTerm = paramValues.SearchTerm,
                SideBar = new SideBarViewModel
                {
                    TownOrPostcode = WebUtility.HtmlEncode(paramValues.Town),
                    DistanceValue = paramValues.Distance,
                    LearningMethod = ConvertStringToFiltersListViewModel(paramValues.LearningMethod),
                    CourseType = ConvertStringToFiltersListViewModel(paramValues.CourseType),
                    Sectors = ConvertStringToFiltersListViewModel(paramValues.Sectors),
                    CourseHours = ConvertStringToFiltersListViewModel(paramValues.CourseHours),
                    CourseStudyTime = ConvertStringToFiltersListViewModel(paramValues.CourseStudyTime),
                    QualificationLevels = string.IsNullOrEmpty(paramValues.QualificationLevels) ? new FiltersListViewModel() : ConvertStringToFiltersListViewModel(paramValues.QualificationLevels),
                    StartDateValue = paramValues.StartDate,
                    CurrentSearchTerm = paramValues.SearchTerm,
                    FiltersApplied = paramValues.FilterA,
                    SelectedOrderByValue = paramValues.OrderByValue,
                    Coordinates = WebUtility.HtmlEncode(paramValues.Coordinates),
                    OrderByOptions = ListFilters.GetOrderByOptions(),
                },
                RequestPage = paramValues.Page,
                IsNewPage = true,
                IsTest = paramValues.IsTest,
                SelectedDistanceValue = paramValues.Distance,
                IsResultBody = true,
                CourseSearchFilters = new CourseSearchFilters
                {
                    CampaignCode = paramValues.CampaignCode,
                },
            };

            if (paramValues.CampaignCode == "LEVEL3_FREE")
            {
                model.FreeCourseSearch = true;
                logService.LogInformation($"{nameof(paramValues.CampaignCode)} has been called");
            }

            var newBodyViewModel = await GenerateModelAsync(model).ConfigureAwait(false);

            try
            {
                model.Results = await findACourseService.GetFilteredData(newBodyViewModel.CourseSearchFilters, newBodyViewModel.CourseSearchOrderBy, model.RequestPage).ConfigureAwait(false);
                model.PageSize = int.TryParse(courseSearchClientSettings.CourseSearchSvcSettings?.SearchPageSize, out int pageSize) ? pageSize : 20;

                foreach (var item in model.Results?.Courses)
                {
                    if (item.Description.Length > 220)
                    {
                        item.Description = item.Description.Substring(0, 200) + "...";
                    }
                }
                string townSearchTerm;

                if (paramValues.Town != null)
                {
                    townSearchTerm = WebUtility.HtmlEncode(paramValues.Town).Replace("&#39;", "%27");
                }
                else
                {
                    townSearchTerm = WebUtility.HtmlEncode(paramValues.Town);
                }


                if (!model.IsTest)
                {
                    TempData["params"] = $"{nameof(paramValues.SearchTerm)}={paramValues.SearchTerm}&" +
                                         $"{nameof(paramValues.Town)}={townSearchTerm}&" +
                                         $"{nameof(paramValues.LearningMethod)}={paramValues.LearningMethod}&" +
                                         $"{nameof(paramValues.CourseType)}={paramValues.CourseType}&" +
                                         $"{nameof(paramValues.Sectors)}={paramValues.Sectors}&" +
                                         $"{nameof(paramValues.CourseHours)}={paramValues.CourseHours}&" +
                                         $"{nameof(paramValues.CourseStudyTime)}={paramValues.CourseStudyTime}&" +
                                         $"{nameof(paramValues.StartDate)}={paramValues.StartDate}&" +
                                         $"{nameof(paramValues.Distance)}={paramValues.Distance}&" +
                                         $"{nameof(paramValues.FilterA)}={paramValues.FilterA}&" +
                                         $"{nameof(paramValues.Page)}={paramValues.Page}&" +
                                         $"{nameof(paramValues.OrderByValue)}={paramValues.OrderByValue}&" +
                                         $"{nameof(paramValues.Coordinates)}={WebUtility.HtmlEncode(paramValues.Coordinates)}&" +
                                         $"{nameof(paramValues.CampaignCode)}={paramValues.CampaignCode}&" +
                                         $"{nameof(paramValues.QualificationLevels)}={paramValues.QualificationLevels}";
                }
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(this.FilterResults)} threw an exception" + ex.Message);
            }

            var viewAsString = await viewHelper.RenderViewAsync(this, "~/Views/Course/_results.cshtml", model, true).ConfigureAwait(false);
            return new AjaxModel
            {
                HTML = viewAsString,
                Count = model.Results?.ResultProperties != null ? model.Results.ResultProperties.TotalResultCount : 0,
                ShowDistanceSelector = newBodyViewModel.CourseSearchFilters.DistanceSpecified,
                UsingAutoSuggestedLocation = newBodyViewModel.UsingAutoSuggestedLocation,
                AutoSuggestedTown = newBodyViewModel.SideBar.TownOrPostcode,
                AutoSuggestedCoordinates = newBodyViewModel.SideBar.Coordinates,
                DidYouMeanLocations = newBodyViewModel.SideBar.DidYouMeanLocations,
            };
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/page")]
        [Route("find-a-course/search/page/body")]
        public Task<IActionResult> Page(ParamValues paramValues, bool isTest)
        {
            logService.LogInformation($"{nameof(this.Page)} has been called");

            if (paramValues == null)
            {
                logService.LogError($"paramValues is null for method: {nameof(Page)} on controller {nameof(CourseController)}");
                return Task.FromResult<IActionResult>(StatusCode((int)HttpStatusCode.NotFound));
            }

            return PageInternalAsync(paramValues, isTest);
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/didyoumeansearchresults")]
        [Route("find-a-course/search/didyoumeansearchresults/body")]
        public async Task<IActionResult> DidYouMeanSearchResults(string searchTerm, string location, bool isFreeCourse = false)
        {
            logService.LogInformation($"{nameof(this.SearchCourse)} has been called");

            if (string.IsNullOrWhiteSpace(searchTerm) && string.IsNullOrWhiteSpace(location))
            {
                logService.LogInformation($"{nameof(searchTerm)} and {nameof(location)} is null or whitespace");
                return Body();
            }

            var model = new BodyViewModel();
            model.SideBar = GetSideBarViewModel();

            if (string.IsNullOrWhiteSpace(location))
            {
                logService.LogInformation($"{nameof(location)} is null or whitespace");
                model.SideBar.Coordinates = null;
                model.SideBar.TownOrPostcode = null;
            }
            else
            {
                //If the user clicked on one of the suggested locations
                var indexOfLocationSpliter = location.IndexOf("|", StringComparison.Ordinal);
                model.SideBar.TownOrPostcode = location.Substring(0, indexOfLocationSpliter);
                model.SideBar.Coordinates = location[(indexOfLocationSpliter + 1)..];
            }

            CourseSearchFilters courseSearchFilters = GetCourseSearchFilters(searchTerm, model.SideBar.TownOrPostcode, model.SideBar.Coordinates);
            if (isFreeCourse)
            {
                logService.LogInformation($"{nameof(isFreeCourse)} has been called");
                courseSearchFilters.CampaignCode = FreeSearchCampaignCode;
                model.FreeCourseSearch = true;
            }

            return await SearchCourses(model, courseSearchFilters, model.SideBar.TownOrPostcode).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/filterresults")]
        [Route("find-a-course/search/filterresults/body")]
        public Task<IActionResult> FilterResults(BodyViewModel model, string location)
        {
            logService.LogInformation($"{nameof(this.FilterResults)} has been called");

            if (model == null)
            {
                logService.LogError($"model is null for method: {nameof(FilterResults)} on controller {nameof(CourseController)}");
                return Task.FromResult<IActionResult>(StatusCode((int)HttpStatusCode.NotFound));
            }

            if (!string.IsNullOrWhiteSpace(model.SideBar.SuggestedLocation) && model.SideBar.SuggestedLocation != model.SideBar.TownOrPostcode)
            {
                //if the user changed the text for the location invalidate the coordinates
                model.SideBar.Coordinates = null;
            }
            else if (!string.IsNullOrWhiteSpace(location))
            {
                //If the user clicked on one of the suggested locations
                var indexOfLocationSpliter = location.IndexOf("|", StringComparison.Ordinal);
                model.SideBar.TownOrPostcode = location.Substring(0, indexOfLocationSpliter);
                model.SideBar.Coordinates = location[(indexOfLocationSpliter + 1)..];
            }

            model.PageSize = int.TryParse(courseSearchClientSettings.CourseSearchSvcSettings?.SearchPageSize, out int pageSize) ? pageSize : 20;

            return FilterResultsInternal(model);
        }

        [HttpGet]
        [Route("api/get/find-a-course/search/{appdata}/isvalidpostcode")]
        public async Task<bool> IsValidPostcode(string appdata)
        {
            var postcode = System.Text.Json.JsonSerializer.Deserialize<string>(appdata);

            if (string.IsNullOrWhiteSpace(postcode))
            {
                throw new ArgumentNullException(nameof(appdata));
            }

            return postcode.IsPostcode();
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/searchcourse")]
        [Route("find-a-course/course/body")]
        [Route("find-a-course/search/searchCourse/body")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> SearchCourse(string searchTerm, string townOrPostcode, string sideBarCoordinates, string sideBarSuggestedLocation)
        {
            logService.LogInformation($"{nameof(this.SearchCourse)} has been called");

            var model = new BodyViewModel();
            CourseSearchFilters courseSearchFilters = GetCourseSearchFilters(searchTerm, townOrPostcode, sideBarCoordinates);

            return await SearchCourses(model, courseSearchFilters, sideBarSuggestedLocation).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("find-a-course/search/searchFreeCourse/body")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> SearchFreeCourse(string searchTerm, string townOrPostcode, string sideBarCoordinates, string sideBarSuggestedLocation)
        {
            logService.LogInformation($"{nameof(this.SearchFreeCourse)} has been called");

            var model = new BodyViewModel();
            CourseSearchFilters courseSearchFilters = GetCourseSearchFilters(searchTerm, townOrPostcode, sideBarCoordinates);
            courseSearchFilters.CampaignCode = FreeSearchCampaignCode;
            model.FreeCourseSearch = true;

            return await SearchCourses(model, courseSearchFilters, sideBarSuggestedLocation).ConfigureAwait(false);
        }

        [HttpGet]
        public IActionResult Results(BodyViewModel model)
        {
            logService.LogInformation($"{nameof(this.Results)} has been called");

            var sideBarViewModel = GetSideBarViewModel();
            foreach (var item in sideBarViewModel.DistanceOptions)
            {
                if (item.Value == model.SideBar.DistanceValue)
                {
                    item.Selected = true;
                    break;
                }
            }

            sideBarViewModel.DistanceValue = model.SideBar.DistanceValue;
            sideBarViewModel.TownOrPostcode = model.SideBar.TownOrPostcode;
            sideBarViewModel.StartDateValue = model.SideBar.StartDateValue;
            sideBarViewModel.DistanceValue = model.SelectedDistanceValue;
            sideBarViewModel.CurrentSearchTerm = model.CurrentSearchTerm;
            sideBarViewModel.FiltersApplied = model.SideBar.FiltersApplied;
            sideBarViewModel.SelectedOrderByValue = model.SideBar.SelectedOrderByValue;
            sideBarViewModel.Coordinates = model.SideBar.Coordinates;
            sideBarViewModel.D = model.SideBar.D;
            sideBarViewModel.DidYouMeanLocations = model.SideBar.DidYouMeanLocations;
            sideBarViewModel.SuggestedLocation = model.SideBar.SuggestedLocation;

            if (model.SideBar.QualificationLevels != null && model.SideBar.QualificationLevels.SelectedIds.Any())
            {
                model.SideBar.QualificationLevels = CheckCheckboxState(model.SideBar.QualificationLevels, sideBarViewModel.QualificationLevels);
                sideBarViewModel.QualificationLevels.SelectedIds = model.SideBar.QualificationLevels.SelectedIds;
            }

            if (model.SideBar.LearningMethod != null && model.SideBar.LearningMethod.SelectedIds.Any())
            {
                model.SideBar.LearningMethod = CheckCheckboxState(model.SideBar.LearningMethod, sideBarViewModel.LearningMethod);
                sideBarViewModel.LearningMethod.SelectedIds = model.SideBar.LearningMethod.SelectedIds;
            }

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.SelectedIds.Any())
            {
                model.SideBar.CourseType = CheckCheckboxState(model.SideBar.CourseType, sideBarViewModel.CourseType);
                sideBarViewModel.CourseType.SelectedIds = model.SideBar.CourseType.SelectedIds;
            }

            if (model.SideBar.Sectors != null && model.SideBar.Sectors.SelectedIds.Any())
            {
                model.SideBar.Sectors = CheckCheckboxState(model.SideBar.Sectors, sideBarViewModel.Sectors);
                sideBarViewModel.Sectors.SelectedIds = model.SideBar.Sectors.SelectedIds;
            }

            if (model.SideBar.CourseHours != null && model.SideBar.CourseHours.SelectedIds.Any())
            {
                model.SideBar.CourseHours = CheckCheckboxState(model.SideBar.CourseHours, sideBarViewModel.CourseHours);
                sideBarViewModel.CourseHours.SelectedIds = model.SideBar.CourseHours.SelectedIds;
            }

            if (model.SideBar.CourseStudyTime != null && model.SideBar.CourseStudyTime.SelectedIds.Any())
            {
                model.SideBar.CourseStudyTime = CheckCheckboxState(model.SideBar.CourseStudyTime, sideBarViewModel.CourseStudyTime);
                sideBarViewModel.CourseStudyTime.SelectedIds = model.SideBar.CourseStudyTime.SelectedIds;
            }

            if (model.Results?.Courses != null && model.Results.Courses.Any())
            {
                foreach (var item in model.Results.Courses)
                {
                    if (item.Description.Length > 220)
                    {
                        item.Description = item.Description.Substring(0, 200) + "...";
                        if (item.Description.Contains("<a href") && !item.Description.Contains("</a>"))
                        {
                            var end = item.Description.IndexOf("<a href");
                            item.Description = item.Description.Substring(0, end) + "...";
                        }
                    }
                }
            }

            var town = model.SideBar.TownOrPostcode;
            var distance = model.SideBar.DistanceValue;
            var learningMethod = model.SideBar.LearningMethod != null && model.SideBar.LearningMethod.SelectedIds?.Count > 0 ? JsonConvert.SerializeObject(model.SideBar.LearningMethod.SelectedIds) : null;
            var courseType = model.SideBar.CourseType != null && model.SideBar.CourseType.SelectedIds?.Count > 0 ? JsonConvert.SerializeObject(model.SideBar.CourseType.SelectedIds) : null;
            var sectors = model.SideBar.Sectors != null && model.SideBar.Sectors.SelectedIds?.Count > 0 ? JsonConvert.SerializeObject(model.SideBar.Sectors.SelectedIds) : null;
            var courseHours = model.SideBar.CourseHours != null && model.SideBar.CourseHours.SelectedIds?.Count > 0 ? JsonConvert.SerializeObject(model.SideBar.CourseHours.SelectedIds) : null;
            var courseStudyTime = model.SideBar.CourseStudyTime != null && model.SideBar.CourseStudyTime?.SelectedIds.Count > 0 ? JsonConvert.SerializeObject(model.SideBar.CourseStudyTime.SelectedIds) : null;
            var startDate = model.SideBar.StartDateValue;
            var searchTerm = sideBarViewModel.CurrentSearchTerm;
            var page = model.RequestPage;
            var filtera = model.SideBar.FiltersApplied;
            var orderByValue = model.SideBar.SelectedOrderByValue;
            var coordinates = model.SideBar.Coordinates;
            var campaignCode = model.CourseSearchFilters?.CampaignCode;
            var qualificationLevels = model.SideBar.QualificationLevels != null && model.SideBar.QualificationLevels?.SelectedIds.Count > 0 ? JsonConvert.SerializeObject(model.SideBar.QualificationLevels.SelectedIds) : null;

            if (!model.IsTest)
            {
                TempData.Remove("params");

                string townSearchTerm;
                if (town != null)
                {
                    townSearchTerm = WebUtility.HtmlEncode(town).Replace("&amp;#39;", "%27");
                    townSearchTerm = townSearchTerm.Replace("&#39;", "%27");
                }
                else
                {
                    townSearchTerm = WebUtility.HtmlEncode(town);
                }

                TempData["params"] = $"{nameof(searchTerm)}={searchTerm}&" +
                                     $"{nameof(town)}={townSearchTerm}&" +
                                     $"{nameof(learningMethod)}={learningMethod}&" +
                                     $"{nameof(courseType)}={courseType}&" +
                                     $"{nameof(sectors)}={sectors}&" +
                                     $"{nameof(courseHours)}={courseHours}&" +
                                     $"{nameof(courseStudyTime)}={courseStudyTime}&" +
                                     $"{nameof(startDate)}={startDate}&" +
                                     $"{nameof(distance)}={distance}&" +
                                     $"{nameof(filtera)}={filtera}&" +
                                     $"{nameof(page)}={page}&" +
                                     $"{nameof(orderByValue)}={orderByValue}&" +
                                     $"{nameof(coordinates)}={WebUtility.HtmlEncode(coordinates)}&" +
                                     $"{nameof(campaignCode)}={campaignCode}&" +
                                     $"{nameof(qualificationLevels)}={qualificationLevels}";
            }

            model.SideBar = sideBarViewModel;
            model.SideBar.OrderByOptions = ListFilters.GetOrderByOptions();
            if (!string.IsNullOrWhiteSpace(model.SideBar.SelectedOrderByValue))
            {
                model.SideBar.OrderByOptions.ForEach(o => o.Selected = false);
                model.SideBar.OrderByOptions.First(x => x.Value == model.SideBar.SelectedOrderByValue).Selected = true;
            }

            model.CourseSearchSettings = courseSearchSettings;
            logService.LogInformation($"{nameof(this.Results)} generated the model and ready to pass to the view");

            return View("Body", model);
        }

        private static CourseSearchFilters GetCourseSearchFilters(string searchTerm, string townOrPostcode, string sideBarCoordinates)
        {
            var courseSearchFilters = new CourseSearchFilters
            {
                CourseType = new List<CourseType> { CourseType.All },
                LearningMethod = new List<LearningMethod> { LearningMethod.All },
                CourseHours = new List<CourseHours> { CourseHours.All },
                StartDate = StartDate.Anytime,
                CourseStudyTime = new List<Fac.AttendancePattern> { Fac.AttendancePattern.Undefined },
                SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? string.Empty : searchTerm,
                Town = string.IsNullOrWhiteSpace(townOrPostcode) ? string.Empty : townOrPostcode,
            };
            if (!string.IsNullOrWhiteSpace(townOrPostcode))
            {
                var locationCoordinates = GetCoordinates(sideBarCoordinates);
                if (locationCoordinates.AreValid)
                {
                    courseSearchFilters.Latitude = locationCoordinates.Latitude;
                    courseSearchFilters.Longitude = locationCoordinates.Longitude;
                    courseSearchFilters.DistanceSpecified = locationCoordinates.AreValid;
                }
            }

            return courseSearchFilters;
        }

        private static LocationCoordinates GetCoordinates(string coordinates)
        {
            var locationCoordinates = new LocationCoordinates() { AreValid = false };
            if (!string.IsNullOrEmpty(coordinates))
            {
                var coordinate = coordinates.Split('|');
                if (coordinate.Length == 2)
                {
                    if (!double.TryParse(coordinate[0], out double convertedDouble))
                    {
                        return locationCoordinates;
                    }

                    locationCoordinates.Longitude = convertedDouble;

                    if (!double.TryParse(coordinate[1], out convertedDouble))
                    {
                        return locationCoordinates;
                    }

                    locationCoordinates.Latitude = convertedDouble;

                    locationCoordinates.AreValid = true;
                }
            }

            return locationCoordinates;
        }

        private static FiltersListViewModel ConvertStringToFiltersListViewModel(string listView)
        {
            var model = new FiltersListViewModel();

            if (listView != null)
            {
                listView = listView.Replace('"', ' ').Replace('[', ' ').Replace(']', ' ').Trim();

                var list = listView.Split(",").Select(x => x.Trim()).ToList();

                model.SelectedIds = list;
            }

            return model;
        }

        private static FiltersListViewModel ConvertStringArrayToFiltersListViewModel(string listView)
        {
            var model = new FiltersListViewModel();

            if (listView != null && listView != "[]")
            {
                listView = listView.Replace('"', ' ').Replace('[', ' ').Replace(']', ' ').Trim();

                var list = listView.Split(",").Select(x => x.Trim()).ToList();

                model.SelectedIds = list;
            }

            return model;
        }

        private static List<T> ConvertToEnumList<T>(List<string> listToConvert)
           where T : struct
        {
            var returnList = new List<T>();

            foreach (var type in listToConvert)
            {
                var removedSpaces = type.Replace(" ", string.Empty);
                Enum.TryParse<T>(removedSpaces, true, out T result);
                returnList.Add(result);
            }

            return returnList;
        }

        private static string NormalizePostcode(string postcode)
        {
            postcode = postcode.Trim();
            postcode = postcode.Replace(" ", string.Empty);

            return postcode.Insert(postcode.Length - 3, " ");
        }

        private async Task<IActionResult> PageInternalAsync(ParamValues paramValues, bool isTest)
        {
            logService.LogInformation($"{nameof(PageInternalAsync)} has been called for {paramValues}");
            var model = new BodyViewModel
            {
                CurrentSearchTerm = paramValues.SearchTerm,
                SideBar = new SideBarViewModel
                {
                    TownOrPostcode = WebUtility.HtmlEncode(paramValues.Town),
                    SuggestedLocation = WebUtility.HtmlEncode(paramValues.Town),
                    DistanceValue = paramValues.Distance,
                    LearningMethod = ConvertStringToFiltersListViewModel(paramValues.LearningMethod),
                    CourseType = ConvertStringToFiltersListViewModel(paramValues.CourseType),
                    Sectors = ConvertStringToFiltersListViewModel(paramValues.Sectors),
                    CourseHours = ConvertStringToFiltersListViewModel(paramValues.CourseHours),
                    CourseStudyTime = ConvertStringToFiltersListViewModel(paramValues.CourseStudyTime),
                    StartDateValue = paramValues.StartDate,
                    CurrentSearchTerm = paramValues.SearchTerm,
                    FiltersApplied = paramValues.FilterA,
                    SelectedOrderByValue = paramValues.OrderByValue,
                    D = paramValues.D,
                    Coordinates = WebUtility.HtmlEncode(paramValues.Coordinates),
                    QualificationLevels = ConvertStringArrayToFiltersListViewModel(paramValues.QualificationLevels),
                },
                RequestPage = paramValues.Page,
                SelectedDistanceValue = paramValues.Distance,
                IsNewPage = true,
                IsTest = isTest,
                FreeCourseSearch = paramValues.CampaignCode == FreeSearchCampaignCode,
                CourseSearchFilters = new CourseSearchFilters
                {
                    CampaignCode = paramValues.CampaignCode,
                },
            };

            logService.LogInformation($"{nameof(this.Page)} generated the model and ready to pass to the view");

            model.FromPaging = true;

            return await FilterResults(model, string.Empty).ConfigureAwait(false);
        }

        private async Task<IActionResult> SearchCourses(BodyViewModel model, CourseSearchFilters filters, string suggestedLocation)
        {
            logService.LogInformation($"{nameof(SearchCourses)} has been called for {filters} and {suggestedLocation}");

            model.SideBar = GetSideBarViewModel();
            model.SideBar.OrderByOptions = ListFilters.GetOrderByOptions();
            model.CurrentSearchTerm = filters?.SearchTerm;
            model.SideBar.CurrentSearchTerm = filters?.SearchTerm;
            model.SideBar.TownOrPostcode = filters?.Town;
            model.RequestPage = 1;
            model.CourseSearchFilters = filters;
            model.PageSize = int.TryParse(courseSearchClientSettings.CourseSearchSvcSettings?.SearchPageSize, out int pageSize) ? pageSize : 20;
            model.SideBar.SelectedOrderByValue = !string.IsNullOrWhiteSpace(model.SideBar.TownOrPostcode) ? CourseSearchOrderBy.Distance.ToString() : CourseSearchOrderBy.Relevance.ToString();
            model.SideBar.SuggestedLocation = suggestedLocation;

            if (!string.IsNullOrWhiteSpace(model.CourseSearchFilters.Town))
            {
                model = await GetSuggestedLocationsAsync(model).ConfigureAwait(false);
                if (!model.UsingAutoSuggestedLocation && model.SideBar.SuggestedLocation != model.SideBar.TownOrPostcode)
                {
                    model.SideBar.Coordinates = null;
                }
                else
                {
                    model.SideBar.Coordinates = string.IsNullOrWhiteSpace(model.SideBar.Coordinates) ? $"{filters.Longitude}|{filters.Latitude}" : model.SideBar.Coordinates;
                    model.CourseSearchFilters.Town = !string.IsNullOrWhiteSpace(model.CourseSearchFilters.Latitude.ToString()) ? null : model.CourseSearchFilters.Town;
                }

                model.SideBar.SuggestedLocation = model.SideBar.TownOrPostcode;
            }

            try
            {
                model.Results = await findACourseService.GetFilteredData(filters, CourseSearchOrderBy.StartDate, 1)
                    .ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(this.SearchCourse)} threw an exception" + ex.Message);
            }

            logService.LogInformation($"{nameof(this.SearchCourse)} generated the model and ready to pass to the view");

            return Results(model);
        }

        private async Task<IActionResult> FilterResultsInternal(BodyViewModel model)
        {
            logService.LogInformation($"{nameof(FilterResultsInternal)} has been called");

            var newBodyViewModel = await GenerateModelAsync(model).ConfigureAwait(false);

            try
            {
                model.Results = await findACourseService.GetFilteredData(newBodyViewModel.CourseSearchFilters, newBodyViewModel.CourseSearchOrderBy, model.RequestPage).ConfigureAwait(false);

                foreach (var item in model.Results.Courses)
                {
                    if (item.Description != null && item.Description.Contains("&lt;a href"))
                    {
                        item.Description = HttpUtility.HtmlDecode(item.Description);
                    }
                }

                model.UsingAutoSuggestedLocation = newBodyViewModel.UsingAutoSuggestedLocation;
                model.SideBar.DidYouMeanLocations = newBodyViewModel.SideBar.DidYouMeanLocations;
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(this.FilterResults)} threw an exception" + ex.Message);
            }

            logService.LogInformation($"{nameof(this.FilterResults)} generated the model and ready to pass to the view");
            return Results(model);
        }

        private async Task<BodyViewModel> GenerateModelAsync(BodyViewModel model)
        {
            logService.LogInformation($"{nameof(GenerateModelAsync)} has been called");
            var courseTypeList = new List<CourseType>();
            var sectorsList = new List<int>();
            var learningMethodList = new List<LearningMethod>();
            var courseHoursList = new List<CourseHours>();
            var courseStudyTimeList = new List<Fac.AttendancePattern>();
            var selectedStartDateValue = StartDate.Anytime;

            if (model.SideBar.LearningMethod != null && model.SideBar.LearningMethod.SelectedIds.Any())
            {
                learningMethodList = ConvertToEnumList<LearningMethod>(model.SideBar.LearningMethod.SelectedIds);
            }

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.SelectedIds.Any())
            {
                courseTypeList = ConvertToEnumList<CourseType>(model.SideBar.CourseType.SelectedIds);
            }

            if (model.SideBar.Sectors != null && model.SideBar.Sectors.SelectedIds.Any())
            {
                sectorsList = model.SideBar.Sectors.SelectedIds
                .Select(s => { int i; return int.TryParse(s, out i) ? i : (int?)null; })
                .Where(i => i.HasValue)
                    .Select(i => i.Value)
                    .ToList();
            }

            if (model.SideBar.CourseHours != null && model.SideBar.CourseHours.SelectedIds.Any())
            {
                courseHoursList = ConvertToEnumList<CourseHours>(model.SideBar.CourseHours.SelectedIds);
            }

            if (model.SideBar.CourseStudyTime != null && model.SideBar.CourseStudyTime.SelectedIds.Any())
            {
                courseStudyTimeList = ConvertToEnumList<Fac.AttendancePattern>(model.SideBar.CourseStudyTime.SelectedIds);
            }

            if (model.SideBar.SelectedOrderByValue != null)
            {
                _ = Enum.TryParse(model.SideBar.SelectedOrderByValue.Replace(" ", string.Empty), true, out CourseSearchOrderBy sortedByCriteria);
                model.CourseSearchOrderBy = sortedByCriteria;
            }
            else
            {
                var sortedByCriteria = CourseSearchOrderBy.Relevance;
                model.CourseSearchOrderBy = sortedByCriteria;
            }

            model.CourseSearchFilters ??= new CourseSearchFilters();

            model.CourseSearchFilters.SearchTerm = model.CurrentSearchTerm;
            model.CourseSearchFilters.LearningMethod = learningMethodList;
            model.CourseSearchFilters.CourseType = courseTypeList;
            model.CourseSearchFilters.SectorIds = sectorsList;
            model.CourseSearchFilters.CourseHours = courseHoursList;
            model.CourseSearchFilters.StartDate = selectedStartDateValue;
            model.CourseSearchFilters.CourseStudyTime = courseStudyTimeList;
            if (model.SideBar.QualificationLevels != null && model.SideBar.QualificationLevels.SelectedIds.Any())
            {
                model.CourseSearchFilters.QualificationLevels = model.SideBar.QualificationLevels.SelectedIds;
            }

            if (model.FreeCourseSearch && string.IsNullOrEmpty(model.CourseSearchFilters.CampaignCode))
            {
                model.CourseSearchFilters.CampaignCode = FreeSearchCampaignCode;
            }

            model.SideBar.FiltersApplied = !model.FromPaging || model.SideBar.FiltersApplied;

            switch (model.SideBar.StartDateValue)
            {
                case "Next 3 months":
                    model.CourseSearchFilters.StartDateTo = DateTime.Today.AddMonths(3);
                    model.CourseSearchFilters.StartDateFrom = DateTime.Today;
                    model.CourseSearchFilters.StartDate = StartDate.SelectDateFrom;
                    break;
                case "In 3 to 6 months":
                    model.CourseSearchFilters.StartDateFrom = DateTime.Today.AddMonths(3);
                    model.CourseSearchFilters.StartDateTo = DateTime.Today.AddMonths(6);
                    model.CourseSearchFilters.StartDate = StartDate.SelectDateFrom;
                    break;
                case "More than 6 months":
                    model.CourseSearchFilters.StartDateFrom = DateTime.Today.AddMonths(6);
                    model.CourseSearchFilters.StartDate = StartDate.SelectDateFrom;
                    break;
                default:
                    model.CourseSearchFilters.StartDate = StartDate.Anytime;
                    break;
            }

            // Added in location parameters
            model = await AddInLocationRequestParametersAsync(model).ConfigureAwait(false);

            // Enter filters criteria here
            model.RequestPage = (model.RequestPage > 1) ? model.RequestPage : 1;

            logService.LogInformation($"{nameof(this.GenerateModelAsync)} generated the model and ready to pass to the view");
            return model;
        }

        private async Task<BodyViewModel> AddInLocationRequestParametersAsync(BodyViewModel model)
        {
            logService.LogInformation($"{nameof(AddInLocationRequestParametersAsync)} has been called");

            float selectedDistanceValue = 10;

            if (model.SelectedDistanceValue != null)
            {
                var resultString = Regex.Match(model.SelectedDistanceValue, @"\d+").Value;
                _ = float.TryParse(resultString, out selectedDistanceValue);
            }

            model.CourseSearchFilters.Distance = selectedDistanceValue;
            if (!string.IsNullOrEmpty(model.SideBar.TownOrPostcode))
            {
                if (model.SideBar.TownOrPostcode.IsPostcode())
                {
                    model.CourseSearchFilters.PostCode = NormalizePostcode(model.SideBar.TownOrPostcode);
                    model.CourseSearchFilters.DistanceSpecified = true;
                }
                else
                {
                    var locationCoordinates = GetCoordinates(model.SideBar.Coordinates);
                    if (locationCoordinates.AreValid)
                    {
                        //using logitutde and latitude
                        model.CourseSearchFilters.Longitude = locationCoordinates.Longitude;
                        model.CourseSearchFilters.Latitude = locationCoordinates.Latitude;
                        model.CourseSearchFilters.DistanceSpecified = true;
                        if (!model.SideBar.TownOrPostcode.Contains(')'))
                        {
                            model = await GetSuggestedLocationsAsync(model).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        //The user has not entered a postcode or a selection from the dropdown.
                        model.CourseSearchFilters.Town = model.SideBar.TownOrPostcode;
                        model = await GetSuggestedLocationsAsync(model).ConfigureAwait(false);
                    }
                }
            }

            model.SideBar.SuggestedLocation = model.SideBar.TownOrPostcode;

            logService.LogInformation($"{nameof(this.AddInLocationRequestParametersAsync)} generated the model and ready to pass to the view");
            return model;
        }

        private async Task<BodyViewModel> GetSuggestedLocationsAsync(BodyViewModel model)
        {

            var town = "";
            var index = -1;
            if (model.SideBar.TownOrPostcode.Contains('('))
            {
                index = model.SideBar.TownOrPostcode.IndexOf(" (");
                town = model.SideBar.TownOrPostcode.Substring(0, index);
            }
            else
            {
                town = model.SideBar.TownOrPostcode;
            }

            logService.LogInformation($"{nameof(GetSuggestedLocationsAsync)} has been called");
            var suggestedLocations = await locationService.GetSuggestedLocationsAsync(WebUtility.HtmlDecode(town)).ConfigureAwait(false);

            if (suggestedLocations.Any())
            {
                var topSuggestion = suggestedLocations.FirstOrDefault();
                if (index > -1)
                {
                    foreach (var location in suggestedLocations)
                    {
                        if (location.LocalAuthorityName.Contains(model.SideBar.TownOrPostcode.Substring(index + 2)))
                        {
                            model.SideBar.TownOrPostcode = $"{location.LocationName} ({location.LocalAuthorityName})";
                            return model;
                        }
                    }
                }
                model.SideBar.TownOrPostcode = $"{topSuggestion.LocationName} ({topSuggestion.LocalAuthorityName})";
                if (model.CourseSearchFilters.Longitude != null)
                {
                    return model;
                }
                model.CourseSearchFilters.Longitude = topSuggestion.Longitude;
                model.CourseSearchFilters.Latitude = topSuggestion.Latitude;
                model.CourseSearchFilters.Distance = 10;
                model.CourseSearchFilters.DistanceSpecified = true;
                model.CourseSearchFilters.Town = null;
                model.SideBar.DidYouMeanLocations = suggestedLocations.Skip(1).Select(x => new LocationSuggestViewModel
                {
                    Label = $"{x.LocationName} ({x.LocalAuthorityName})",
                    Value = $"{x.Longitude}|{x.Latitude}",
                }).ToList();
                model.SideBar.Coordinates = $"{topSuggestion.Longitude}|{topSuggestion.Latitude}";
                model.UsingAutoSuggestedLocation = true;
            }

            logService.LogInformation($"{nameof(this.GetSuggestedLocationsAsync)} generated the model and ready to pass to the view");
            return model;
        }

        private FiltersListViewModel MapFilter(string text, string title, List<Filter> lstFilter)
        {
            logService.LogInformation($"{nameof(this.MapFilter)} has been called for {title}");

            var filterModel = new FiltersListViewModel
            {
                FilterText = text,
                FilterTitle = title,
            };

            var checkboxList = new List<CheckBoxItem>();
            foreach (var item in lstFilter)
            {
                checkboxList.Add(new CheckBoxItem(item.Id, item.Text, false, false));
            }

            filterModel.LstChkFilter = checkboxList;

            logService.LogInformation($"{nameof(this.MapFilter)} {title} list has been generated successfully");

            return filterModel;
        }

        private FiltersListViewModel CheckCheckboxState(FiltersListViewModel model, FiltersListViewModel newModel)
        {
            logService.LogInformation($"{nameof(this.CheckCheckboxState)} has been called");

            foreach (var item in newModel.LstChkFilter)
            {
                var exists = model.SelectedIds.Contains(item.Id);
                if (exists)
                {
                    item.IsChecked = true;
                }
            }

            logService.LogInformation($"{nameof(this.CheckCheckboxState)} generated the model and ready to pass to the view");
            return model;
        }

        [ResponseCache(Duration = 43200)]
        private SideBarViewModel GetSideBarViewModel()
        {
            logService.LogInformation($"{nameof(GetSideBarViewModel)} has been called");

            var sideBarViewModel = new SideBarViewModel
            {
                CourseType = MapFilter("courseType", "Course type", ListFilters.GetCourseTypeList()),
                Sectors = MapFilter("sectors", "Sectors", GetSectorsList().Result),
                LearningMethod = MapFilter("learningMethod", "Learning method", ListFilters.GetLearningMethodList()),
                CourseHours = MapFilter("courseHours", "Course hours", ListFilters.GetHoursList()),
                CourseStudyTime = MapFilter("courseStudyTime", "Course study time", ListFilters.GetStudyTimeList()),
                QualificationLevels = MapFilter("qualificationLevels", "Course qualification level", ListFilters.GetLevelList()),
                StartDateOptions = ListFilters.GetStartDateList(),
                DistanceOptions = ListFilters.GetDistanceList(),
            };

            logService.LogInformation($"{nameof(this.GetSideBarViewModel)} generated the model and ready to pass to the view");
            return sideBarViewModel;
        }

        private async Task<List<Filter>> GetSectorsList()
        {
            var sectors = await findACourseService.GetSectors();

            return sectors?.Select(s => new Filter { Id = s.Id.ToString(), Text = s.Description }).ToList() ?? new List<Filter>();
        }
    }
}