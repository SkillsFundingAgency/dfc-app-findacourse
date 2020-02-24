﻿using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Data.Helpers;
using DFC.App.FindACourse.Extensions;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.CompositeInterfaceModels.FindACourseClient;
using GdsCheckboxList.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CourseController> logger;
        private readonly IFindACourseService findACourseService;

        public CourseController(ILogger<CourseController> logger, IFindACourseService findACourseService)
        {
            this.logger = logger;
            this.findACourseService = findACourseService;
            this.SearchCourse(string.Empty);
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new IndexViewModel
            {
                Documents = new List<IndexDocumentViewModel> { new IndexDocumentViewModel { CanonicalName = "Index" } },
            };

            return this.NegotiateContentResult(viewModel);
        }

        [HttpGet]
        public IActionResult Document()
        {
            this.logger.LogInformation($"{nameof(this.Document)} has been called");

            var model = new DocumentViewModel
            {
                Title = "Find a Course",
                BreadcrumbTitle = "Breadcrumb 1",
                Keywords = "fac",
                CanonicalName = "find-a-course",
                Content = new HtmlString("Document Content"),
                IncludeInSitemap = true,
            };

            this.logger.LogInformation($"{nameof(this.Document)} generated the model and ready to pass to the view");

            return this.View(model);
        }

        [HttpGet]
        public IActionResult Head()
        {
            this.logger.LogInformation($"{nameof(this.Head)} has been called");

            var model = new HeadViewModel { Title = "Find a Course", Description = "FAC", Keywords = "fac", CanonicalUrl = "find-a-course" };

            this.logger.LogInformation($"{nameof(this.Head)} generated the model and ready to pass to the view");

            return View(model);
        }

        [HttpGet]
        [Route("find-a-course/{**data}")]
        public IActionResult HeroBanner(string data)
        {
            this.logger.LogInformation($"{nameof(this.HeroBanner)} has been called");
            var returnView = string.Empty;

            var lastPartofUrl = data.Split("/").Last();

            if (lastPartofUrl != "details")
            {
                returnView = "~/Views/Shared/herobanner.cshtml";
            }
            else
            {
                returnView = "~/Views/Shared/detailsHeroBanner.cshtml";
            }

            return View(returnView);
        }

        [HttpGet]
        public IActionResult Breadcrumb()
        {
            this.logger.LogInformation($"{nameof(this.Breadcrumb)} has been called");

            var model = new BreadcrumbViewModel
            {
                Paths = new List<BreadcrumbPathViewModel>
                {
                    new BreadcrumbPathViewModel { Route = "/", Title = "Home", AddHyperlink = true },
                    new BreadcrumbPathViewModel { Route = "/find-a-course", Title = "Find a Course", AddHyperlink = true },
                },
            };

            this.logger.LogInformation($"{nameof(this.Breadcrumb)} generated the model and ready to pass to the view");
            return View(model);
        }

        [HttpGet]
        [Route("find-a-course/{controller}/bodytop/{**data}")]
        public IActionResult BodyTop()
        {
            this.logger.LogInformation($"{nameof(this.BodyTop)} has been called");

            return View();
        }

        [HttpGet]
        [Route("find-a-course/{controller}/body/{**data}")]
        public IActionResult Body()
        {
            this.logger.LogInformation($"{nameof(this.Body)} has been called");

            var model = new BodyViewModel();

            model = new BodyViewModel { Content = new HtmlString("Find a course: Body element") };
            model.SideBar = this.GetSideBarViewModel();
            model.OrderByOptions = ListFilters.GetOrderByOptions();

            this.logger.LogInformation($"{nameof(this.Body)} generated the model and ready to pass to the view");

            return View(model);
        }

        [HttpGet]
        [Route("find-a-course/{controller}/bodyfooter/{**data}")]
        public IActionResult BodyFooter()
        {
            this.logger.LogInformation($"{nameof(this.BodyFooter)} has been called");

            return this.NoContent();
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/page")]
        public async Task<IActionResult> Page(string searchTerm, string town, string distance, string courseType, string courseHours, string studyTime, string startDate, int page)
        {
            this.logger.LogInformation($"{nameof(this.Page)} has been called");

            var model = new BodyViewModel
            {
                CurrentSearchTerm = searchTerm,
                SideBar = new SideBarViewModel
                {
                    TownOrPostcode = town,
                    DistanceValue = distance,
                    CourseType = this.ConvertStringToFiltersListViewModel(courseType),
                    CourseHours = this.ConvertStringToFiltersListViewModel(courseHours),
                    CourseStudyTime = this.ConvertStringToFiltersListViewModel(studyTime),
                    StartDate = this.ConvertStringToFiltersListViewModel(startDate),
                },
                RequestPage = page,
            };

            this.logger.LogInformation($"{nameof(this.Page)} generated the model and ready to pass to the view");

            return await this.FilterResults(model).ConfigureAwait(true);
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/filterresults")]
        public async Task<IActionResult> FilterResults(BodyViewModel model)
        {
            this.logger.LogInformation($"{nameof(this.FilterResults)} has been called");

            var courseTypeList = new List<CourseType>();
            var courseHoursList = new List<CourseHours>();
           // var courseStartDateList = new List<StartDate>();
            var courseStudyTimeList = new List<Fac.AttendancePattern>();

            float selectedDistanceValue = 10;

            if (model?.SideBar.DistanceOptions != null)
            {
                _ = float.TryParse(model.SideBar.DistanceValue, out selectedDistanceValue);

                // Enum.TryParse(model, out CourseSearchOrderBy sortedByCriteria);
            }

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.SelectedIds.Any())
            {
                courseTypeList = this.ConvertToEnumList<CourseType>(model.SideBar.CourseType.SelectedIds);
            }

            if (model.SideBar.CourseHours != null && model.SideBar.CourseHours.SelectedIds.Any())
            {
                courseHoursList = this.ConvertToEnumList<CourseHours>(model.SideBar.CourseHours.SelectedIds);
            }

            if (model.SideBar.StartDate != null && model.SideBar.StartDate.SelectedIds.Any())
            {
                //ADDITIONAL LOGIC TO BE PERFORMED HERE.  NEEDS TO GO IN NEW TICKET
                //courseStartDateList = this.ConvertToEnumList<StartDate>(model.SideBar.StartDate.SelectedIds);
            }

            if (model.SideBar.CourseStudyTime != null && model.SideBar.CourseStudyTime.SelectedIds.Any())
            {

                courseStudyTimeList = this.ConvertToEnumList<Fac.AttendancePattern>(model.SideBar.CourseStudyTime.SelectedIds);
            }

            var courseSearchFilters = new CourseSearchFilters
            {
                SearchTerm = model.CurrentSearchTerm,
                Distance = selectedDistanceValue,
                DistanceSpecified = true,
                CourseType = courseTypeList,
                CourseHours = courseHoursList,
                StartDate = StartDate.Anytime, //WAITING ON ADDITIONAL LOGIC TO BE DEFINED
                CourseStudyTime = courseStudyTimeList,
            };

            // Enter filters criteria here
            model.RequestPage = (model.RequestPage > 1) ? model.RequestPage : 1;

            try
            {
                model.Results = await this.findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.Relevance, model.RequestPage).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"{nameof(this.FilterResults)} threw an exception", ex.Message);
            }

            this.logger.LogInformation($"{nameof(this.FilterResults)} generated the model and ready to pass to the view");

            return await this.Results(model).ConfigureAwait(true);
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/searchcourse")]
        public async Task<IActionResult> SearchCourse(string searchTerm)
        {
            this.logger.LogInformation($"{nameof(this.SearchCourse)} has been called");

            var model = new BodyViewModel();
            var courseSearchFilters = new CourseSearchFilters
            {
                CourseType = new List<CourseType> { CourseType.All },
                CourseHours = new List<CourseHours> { CourseHours.All },
                StartDate = StartDate.Anytime, //new List<StartDate> { StartDate.Anytime },
                CourseStudyTime = new List<Fac.AttendancePattern> { Fac.AttendancePattern.Undefined },
                SearchTerm = string.IsNullOrEmpty(searchTerm) ? string.Empty : searchTerm,
            };

            model.SideBar = this.GetSideBarViewModel();
            model.OrderByOptions = ListFilters.GetOrderByOptions();
            model.CurrentSearchTerm = searchTerm;
            model.RequestPage = 1;

            try
            {
                model.Results = await this.findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.Relevance, 1).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"{nameof(this.SearchCourse)} threw an exception", ex.Message);
            }

            this.logger.LogInformation($"{nameof(this.SearchCourse)} generated the model and ready to pass to the view");

            return await this.Results(model).ConfigureAwait(true);
        }

        [HttpGet]
        [NonAction]
        public async Task<IActionResult> Results(BodyViewModel model)
        {
            this.logger.LogInformation($"{nameof(this.Results)} has been called");

            var sideBarViewModel = this.GetSideBarViewModel();
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

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.SelectedIds.Any())
            {
                model.SideBar.CourseType = this.CheckCheckboxState(model.SideBar.CourseType, sideBarViewModel.CourseType);
                sideBarViewModel.CourseType.SelectedIds = model.SideBar.CourseType.SelectedIds;
            }

            if (model.SideBar.CourseHours != null && model.SideBar.CourseHours.SelectedIds.Any())
            {
                model.SideBar.CourseHours = this.CheckCheckboxState(model.SideBar.CourseHours, sideBarViewModel.CourseHours);
                sideBarViewModel.CourseHours.SelectedIds = model.SideBar.CourseHours.SelectedIds;
            }

            if (model.SideBar.CourseStudyTime != null && model.SideBar.CourseStudyTime.SelectedIds.Any())
            {
                model.SideBar.CourseStudyTime = this.CheckCheckboxState(model.SideBar.CourseStudyTime, sideBarViewModel.CourseStudyTime);
                sideBarViewModel.CourseStudyTime.SelectedIds = model.SideBar.CourseStudyTime.SelectedIds;
            }

            if (model.SideBar.StartDate != null && model.SideBar.StartDate.SelectedIds.Any())
            {
                model.SideBar.StartDate = this.CheckCheckboxState(model.SideBar.StartDate, sideBarViewModel.StartDate);
                sideBarViewModel.StartDate.SelectedIds = model.SideBar.StartDate.SelectedIds;
            }

            foreach (var item in model.Results.Courses)
            {
                if (item.Description.Length > 220)
                {
                    item.Description = item.Description.Substring(0, 200);
                    item.Description += "...";
                }
            }

            model.SideBar = sideBarViewModel;
            model.OrderByOptions = ListFilters.GetOrderByOptions();

            this.logger.LogInformation($"{nameof(this.Results)} generated the model and ready to pass to the view");

            return View("Body", model);
        }

        private FiltersListViewModel MapFilter(string text, string title, List<Filter> lstFilter)
        {
            this.logger.LogInformation($"{nameof(this.MapFilter)} has been called for {title}");

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

            this.logger.LogInformation($"{nameof(this.MapFilter)} {title} list has been generated successfully");

            return filterModel;
        }

        private FiltersListViewModel CheckCheckboxState(FiltersListViewModel model, FiltersListViewModel newModel)
        {
            this.logger.LogInformation($"{nameof(this.CheckCheckboxState)} has been called");

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

        private FiltersListViewModel ConvertStringToFiltersListViewModel(string listView)
        {
            var model = new FiltersListViewModel();

            listView = listView.Replace('"', ' ').Replace('[', ' ').Replace(']', ' ').Trim();

            var list = listView.Split(",").ToList().Select(x => x.Trim()).ToList();

            model.SelectedIds = list;
            return model;
        }

        private SideBarViewModel GetSideBarViewModel()
        {
            var sideBarViewModel = new SideBarViewModel
            {
                CourseType = this.MapFilter("courseType", "Course type", ListFilters.GetCourseTypeList()),
                CourseHours = this.MapFilter("courseHours", "Course hours", ListFilters.GetHoursList()),
                CourseStudyTime = this.MapFilter("courseStudyTime", "Course study time", ListFilters.GetStudyTimeList()),
                StartDate = this.MapFilter("courseStartDate", "Start date", ListFilters.GetStartDateList()),
                DistanceOptions = ListFilters.GetDistanceList(),
            };

            return sideBarViewModel;
        }

        private List<T> ConvertToEnumList<T>(List<string> listToConvert) where T : struct
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
    }
}