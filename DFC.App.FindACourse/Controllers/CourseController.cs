﻿using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Data.Helpers;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Extensions;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fac = DFC.FindACourseClient;

namespace DFC.App.FindACourse.Controllers
{
    public class CourseController : Controller
    {
        private readonly ILogService logService;
        private readonly IFindACourseService findACourseService;

        public CourseController(ILogService logService, IFindACourseService findACourseService)
        {
            this.logService = logService;
            this.findACourseService = findACourseService;
        }

        [HttpGet]
        [Route("search/")]
        public IActionResult Index()
        {
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
                CanonicalName = "find-a-course",
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

            var model = new HeadViewModel { Title = $"{title} | Find a course | National Careers Service", Description = "FAC", Keywords = "fac", CanonicalUrl = "find-a-course" };

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
        [Route("find-a-course/search/{articleName}/body")]
        [Route("find-a-course/search/body")]
        [ResponseCache(Duration = 43200)]
        public async Task<IActionResult> Body(string articleName)
        {
            logService.LogInformation($"{nameof(this.Body)} has been called");

            var model = new BodyViewModel { Content = new HtmlString("Find a course: Body element") };
            model.SideBar = GetSideBarViewModel();
            model.SideBar.OrderByOptions = ListFilters.GetOrderByOptions();

            logService.LogInformation($"{nameof(this.Body)} generated the model and ready to pass to the view");

            return await SearchCourse(string.Empty).ConfigureAwait(true);
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
            var paramValues = System.Text.Json.JsonSerializer.Deserialize<ParamValues>(appData);
            bool? isPostcode = null;

            if (paramValues == null)
            {
                throw new ArgumentNullException(nameof(appData));
            }

            var model = new BodyViewModel
            {
                CurrentSearchTerm = paramValues.SearchTerm,
                SideBar = new SideBarViewModel
                {
                    TownOrPostcode = paramValues.Town,
                    DistanceValue = paramValues.Distance,
                    CourseType = ConvertStringToFiltersListViewModel(paramValues.CourseType),
                    CourseHours = ConvertStringToFiltersListViewModel(paramValues.CourseHours),
                    CourseStudyTime = ConvertStringToFiltersListViewModel(paramValues.CourseStudyTime),
                    StartDateValue = paramValues.StartDate,
                    CurrentSearchTerm = paramValues.SearchTerm,
                    FiltersApplied = paramValues.FilterA,
                    SelectedOrderByValue = paramValues.OrderByValue,
                },
                RequestPage = paramValues.Page,
                IsNewPage = true,
                IsTest = paramValues.IsTest,
                SelectedDistanceValue = paramValues.Distance,
                IsResultBody = true,
            };

            var newBodyViewModel = GenerateModel(model);

            try
            {
                model.Results = await findACourseService.GetFilteredData(newBodyViewModel.CourseSearchFilters, newBodyViewModel.CourseSearchOrderBy, model.RequestPage).ConfigureAwait(false);
                foreach (var item in model.Results.Courses)
                {
                    if (item.Description.Length > 220)
                    {
                        item.Description = item.Description.Substring(0, 200) + "...";
                    }
                }

                isPostcode = !string.IsNullOrEmpty(paramValues.Town) ? (bool?)paramValues.Town.IsPostcode() : null;

                if (!model.IsTest)
                {
                    TempData["params"] = $"{nameof(paramValues.SearchTerm)}={paramValues.SearchTerm}&" +
                                         $"{nameof(paramValues.Town)}={paramValues.Town}&" +
                                         $"{nameof(paramValues.CourseType)}={paramValues.CourseType}&" +
                                         $"{nameof(paramValues.CourseHours)}={paramValues.CourseHours}&" +
                                         $"{nameof(paramValues.CourseStudyTime)}={paramValues.CourseStudyTime}&" +
                                         $"{nameof(paramValues.StartDate)}={paramValues.StartDate}&" +
                                         $"{nameof(paramValues.Distance)}={paramValues.Distance}&" +
                                         $"{nameof(paramValues.FilterA)}={paramValues.FilterA}&" +
                                         $"{nameof(paramValues.Page)}={paramValues.Page}&" +
                                         $"{nameof(paramValues.OrderByValue)}={paramValues.OrderByValue}";
                }
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(this.FilterResults)} threw an exception" + ex.Message);
            }

            var viewAsString = await this.RenderViewAsync("~/Views/Course/_results.cshtml", model, true).ConfigureAwait(false);
            return new AjaxModel { HTML = viewAsString, Count = model.Results?.ResultProperties != null ? model.Results.ResultProperties.TotalResultCount : 0, IsPostcode = isPostcode };
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/page")]
        [Route("find-a-course/search/page/body")]
        public async Task<IActionResult> Page(ParamValues paramValues, bool isTest)
        {
            logService.LogInformation($"{nameof(this.Page)} has been called");

            var isPostcode = !string.IsNullOrEmpty(paramValues.Town) ? (bool?)paramValues.Town.IsPostcode() : null;
            paramValues.D = isPostcode.HasValue && isPostcode.Value ? 1 : 0;

            var model = new BodyViewModel
            {
                CurrentSearchTerm = paramValues.SearchTerm,
                SideBar = new SideBarViewModel
                {
                    TownOrPostcode = paramValues.Town,
                    DistanceValue = paramValues.Distance,
                    CourseType = ConvertStringToFiltersListViewModel(paramValues.CourseType),
                    CourseHours = ConvertStringToFiltersListViewModel(paramValues.CourseHours),
                    CourseStudyTime = ConvertStringToFiltersListViewModel(paramValues.CourseStudyTime),
                    StartDateValue = paramValues.StartDate,
                    CurrentSearchTerm = paramValues.SearchTerm,
                    FiltersApplied = paramValues.FilterA,
                    SelectedOrderByValue = paramValues.OrderByValue,
                    D = paramValues.D,
                },
                RequestPage = paramValues.Page,
                SelectedDistanceValue = paramValues.Distance,
                IsNewPage = true,
                IsTest = isTest,
            };

            logService.LogInformation($"{nameof(this.Page)} generated the model and ready to pass to the view");

            model.FromPaging = true;

            return await FilterResults(model).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/filterresults")]
        [Route("find-a-course/search/filterresults/body")]
        public async Task<IActionResult> FilterResults(BodyViewModel model)
        {
            logService.LogInformation($"{nameof(this.FilterResults)} has been called");

            var newBodyViewModel = GenerateModel(model);

            try
            {
                model.Results = await findACourseService.GetFilteredData(newBodyViewModel.CourseSearchFilters, newBodyViewModel.CourseSearchOrderBy, model.RequestPage).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(this.FilterResults)} threw an exception" + ex.Message);
            }

            logService.LogInformation($"{nameof(this.FilterResults)} generated the model and ready to pass to the view");

            return Results(model);
        }

        [HttpGet]
        [Route("api/get/find-a-course/search/{appdata}/isvalidpostcode")]
        public async Task<bool> IsValidPostcode(string appdata)
        {
            var postcode = System.Text.Json.JsonSerializer.Deserialize<string>(appdata);

            if (postcode == null)
            {
                throw new ArgumentNullException(nameof(appdata));
            }

            return postcode.IsPostcode();
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/searchcourse")]
        [Route("find-a-course/course/body")]
        [Route("find-a-course/search/searchCourse/body")]
        public async Task<IActionResult> SearchCourse(string searchTerm)
        {
            logService.LogInformation($"{nameof(this.SearchCourse)} has been called");

            var model = new BodyViewModel();
            var courseSearchFilters = new CourseSearchFilters
            {
                CourseType = new List<CourseType> { CourseType.All },
                CourseHours = new List<CourseHours> { CourseHours.All },
                StartDate = StartDate.Anytime,
                CourseStudyTime = new List<Fac.AttendancePattern> { Fac.AttendancePattern.Undefined },
                SearchTerm = string.IsNullOrEmpty(searchTerm) ? string.Empty : searchTerm,
            };

            model.SideBar = GetSideBarViewModel();
            model.SideBar.OrderByOptions = ListFilters.GetOrderByOptions();
            model.CurrentSearchTerm = searchTerm;
            model.SideBar.CurrentSearchTerm = searchTerm;
            model.RequestPage = 1;

            try
            {
                model.Results = await findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.StartDate, 1).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                logService.LogError($"{nameof(this.SearchCourse)} threw an exception" + ex.Message);
            }

            logService.LogInformation($"{nameof(this.SearchCourse)} generated the model and ready to pass to the view");

            return Results(model);
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
            sideBarViewModel.D = model.SideBar.D;

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.SelectedIds.Any())
            {
                model.SideBar.CourseType = CheckCheckboxState(model.SideBar.CourseType, sideBarViewModel.CourseType);
                sideBarViewModel.CourseType.SelectedIds = model.SideBar.CourseType.SelectedIds;
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
                    }
                }
            }

            var town = model.SideBar.TownOrPostcode;
            var distance = model.SideBar.DistanceValue;
            var courseType = model.SideBar.CourseType != null && model.SideBar.CourseType.SelectedIds?.Count > 0 ? JsonConvert.SerializeObject(model.SideBar.CourseType.SelectedIds) : null;
            var courseHours = model.SideBar.CourseHours != null && model.SideBar.CourseHours.SelectedIds?.Count > 0 ? JsonConvert.SerializeObject(model.SideBar.CourseHours.SelectedIds) : null;
            var courseStudyTime = model.SideBar.CourseStudyTime != null && model.SideBar.CourseStudyTime?.SelectedIds.Count > 0 ? JsonConvert.SerializeObject(model.SideBar.CourseStudyTime.SelectedIds) : null;
            var startDate = model.SideBar.StartDateValue;
            var searchTerm = sideBarViewModel.CurrentSearchTerm;
            var page = model.RequestPage;
            var filtera = model.SideBar.FiltersApplied;
            var orderByValue = model.SideBar.SelectedOrderByValue;

            if (!model.IsTest)
            {
                TempData["params"] = $"{nameof(searchTerm)}={searchTerm}&" +
                                     $"{nameof(town)}={town}&" +
                                     $"{nameof(courseType)}={courseType}&" +
                                     $"{nameof(courseHours)}={courseHours}&" +
                                     $"{nameof(courseStudyTime)}={courseStudyTime}&" +
                                     $"{nameof(startDate)}={startDate}&" +
                                     $"{nameof(distance)}={distance}&" +
                                     $"{nameof(filtera)}={filtera}&" +
                                     $"{nameof(page)}={page}&" +
                                     $"{nameof(orderByValue)}={orderByValue}";
            }

            model.SideBar = sideBarViewModel;
            model.SideBar.OrderByOptions = ListFilters.GetOrderByOptions();

            logService.LogInformation($"{nameof(this.Results)} generated the model and ready to pass to the view");

            return View("Body", model);
        }

        private static BodyViewModel GenerateModel(BodyViewModel model)
        {
            var courseTypeList = new List<CourseType>();
            var courseHoursList = new List<CourseHours>();
            var courseStudyTimeList = new List<Fac.AttendancePattern>();
            var selectedStartDateValue = StartDate.Anytime;

            float selectedDistanceValue = 10;

            if (model.SelectedDistanceValue != null)
            {
                var resultString = Regex.Match(model.SelectedDistanceValue, @"\d+").Value;
                _ = float.TryParse(resultString, out selectedDistanceValue);
            }

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.SelectedIds.Any())
            {
                courseTypeList = ConvertToEnumList<CourseType>(model.SideBar.CourseType.SelectedIds);
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

            var courseSearchFilters = new CourseSearchFilters
            {
                SearchTerm = model.CurrentSearchTerm,
                CourseType = courseTypeList,
                CourseHours = courseHoursList,
                StartDate = selectedStartDateValue,
                CourseStudyTime = courseStudyTimeList,
                Distance = selectedDistanceValue,
            };

            model.SideBar.FiltersApplied = model.FromPaging ? model.SideBar.FiltersApplied : true;

            switch (model.SideBar.StartDateValue)
            {
                case "Next 3 months":
                    courseSearchFilters.StartDateTo = DateTime.Today.AddMonths(3);
                    courseSearchFilters.StartDateFrom = DateTime.Today;
                    courseSearchFilters.StartDate = StartDate.SelectDateFrom;
                    break;
                case "In 3 to 6 months":
                    courseSearchFilters.StartDateFrom = DateTime.Today.AddMonths(3);
                    courseSearchFilters.StartDateTo = DateTime.Today.AddMonths(6);
                    courseSearchFilters.StartDate = StartDate.SelectDateFrom;
                    break;
                case "More than 6 months":
                    courseSearchFilters.StartDateFrom = DateTime.Today.AddMonths(6);
                    courseSearchFilters.StartDate = StartDate.SelectDateFrom;
                    break;
                default:
                    courseSearchFilters.StartDate = StartDate.Anytime;
                    break;
            }

            if (!string.IsNullOrEmpty(model.SideBar.TownOrPostcode))
            {
                if (model.SideBar.TownOrPostcode.IsPostcode())
                {
                    courseSearchFilters.PostCode = NormalizePostcode(model.SideBar.TownOrPostcode);
                    courseSearchFilters.Distance = selectedDistanceValue;
                    courseSearchFilters.DistanceSpecified = true;
                }
                else
                {
                    courseSearchFilters.Town = "\u0022" + model.SideBar.TownOrPostcode + "\u0022";
                }
            }

            // Enter filters criteria here
            model.RequestPage = (model.RequestPage > 1) ? model.RequestPage : 1;

            model.CourseSearchFilters = courseSearchFilters;

            return model;
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

            return model;
        }

        [ResponseCache(Duration = 43200)]
        private SideBarViewModel GetSideBarViewModel()
        {
            var sideBarViewModel = new SideBarViewModel
            {
                CourseType = MapFilter("courseType", "Course type", ListFilters.GetCourseTypeList()),
                CourseHours = MapFilter("courseHours", "Course hours", ListFilters.GetHoursList()),
                CourseStudyTime = MapFilter("courseStudyTime", "Course study time", ListFilters.GetStudyTimeList()),
                StartDateOptions = ListFilters.GetStartDateList(),
                DistanceOptions = ListFilters.GetDistanceList(),
            };

            return sideBarViewModel;
        }
    }
}