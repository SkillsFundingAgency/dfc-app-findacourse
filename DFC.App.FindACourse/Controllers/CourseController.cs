using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Data.Helpers;
using DFC.App.FindACourse.Extensions;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.FindACourseClient;
using GdsCheckboxList.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        }

        [HttpGet]
        public async Task<IActionResult> Index()
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

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Head()
        {
            this.logger.LogInformation($"{nameof(this.Head)} has been called");

            var model = new HeadViewModel { Title = "Find a Course", Description = "FAC", Keywords = "fac", CanonicalUrl = "find-a-course" };

            this.logger.LogInformation($"{nameof(this.Head)} generated the model and ready to pass to the view");

            return View(model);
        }

        [HttpGet]
        [Route("find-a-course/{controller}/herobanner/{**data}")]
        public async Task<IActionResult> HeroBanner()
        {
            this.logger.LogInformation($"{nameof(this.HeroBanner)} has been called");

            return View();
        }

        public async Task<IActionResult> Breadcrumb()
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
        public async Task<IActionResult> BodyTop()
        {
            this.logger.LogInformation($"{nameof(this.BodyTop)} has been called");

            return View();
        }

        [HttpGet]
        [Route("find-a-course/{controller}/body/{**data}")]
        public async Task<IActionResult> Body()
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
        public IActionResult BodyFooter(string article)
        {
            this.logger.LogInformation($"{nameof(this.BodyFooter)} has been called");

            return this.NoContent();
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/page")]
        public async Task<IActionResult> Page(string searchTerm, string town, string distance, string courseType, string courseHours, string studyTime, string startDate, int page)
        {
            this.logger.LogInformation($"{nameof(this.Page)} has been called");

            var model = new BodyViewModel();
            model.CurrentSearchTerm = searchTerm;
            model.SideBar = new SideBarViewModel();
            model.SideBar.TownOrPostcode = town;
            model.SideBar.DistanceValue = distance;
            model.SideBar.CourseType = this.ConvertStringToFiltersListViewModel(courseType);
            model.SideBar.CourseHours = this.ConvertStringToFiltersListViewModel(courseHours);
            model.SideBar.CourseStudyTime = this.ConvertStringToFiltersListViewModel(studyTime);
            model.SideBar.StartDate = this.ConvertStringToFiltersListViewModel(startDate);
            model.RequestPage = page;

            this.logger.LogInformation($"{nameof(this.Page)} generated the model and ready to pass to the view");

            return await this.FilterResults(model).ConfigureAwait(true);
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/filterresults")]
        public async Task<IActionResult> FilterResults(BodyViewModel model)
        {
            this.logger.LogInformation($"{nameof(this.FilterResults)} has been called");

            CourseType courseTypeCriteria = CourseType.All;
            CourseHours courseHoursCriteria = CourseHours.All;
            StartDate courseStartDateCriteria = StartDate.Anytime;
            float selectedDistanceValue = 10;

            if (model?.SideBar.DistanceOptions != null)
            {
                _ = float.TryParse(model.SideBar.DistanceValue, out selectedDistanceValue);
                //Enum.TryParse(model, out CourseSearchOrderBy sortedByCriteria);
            }

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.selectedIds.Any())
            {
                _ = Enum.TryParse(model.SideBar.CourseType.selectedIds[0], out courseTypeCriteria);
            }

            if (model.SideBar.CourseHours != null && model.SideBar.CourseHours.selectedIds.Any())
            {

                _ = Enum.TryParse(model.SideBar.CourseHours.selectedIds[0], out courseHoursCriteria);
            }


            if (model.SideBar.StartDate != null && model.SideBar.StartDate.selectedIds.Any())
            {
                _ = Enum.TryParse(model.SideBar.StartDate.selectedIds[0], out courseStartDateCriteria);
            }

            //  TODO - Uncomment the line below after it has been added to the nuget package
            //  Enum.TryParse(model.SideBar.CourseStudyTime.selectedIds[0], out StartDate courseStudyTimeCriteria);

            var courseSearchFilters = new CourseSearchFilters();
            courseSearchFilters.SearchTerm = model.CurrentSearchTerm;
            courseSearchFilters.Distance = selectedDistanceValue;
            courseSearchFilters.DistanceSpecified = true;
            //  TODO: FOLLOWING NEED TO BE LISTS - NEED TO UPDATE NUGET PACKAGE
            courseSearchFilters.CourseType = courseTypeCriteria;
            courseSearchFilters.CourseHours = courseHoursCriteria;
            courseSearchFilters.StartDate = courseStartDateCriteria;

            //  TODO: FOLLOWING DOES NOT YET EXIST IN THE NUGET PACKAGE
            //  courseSearchFilters.StudyTime = courseStudyTimeCriteria;

            //  Enter filters criteria here
            model.RequestPage = (model.RequestPage > 1) ? model.RequestPage : 1;

            try
            {
                model.Results = await this.findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.Relevance, model.RequestPage);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"{nameof(this.FilterResults)} threw an exception", ex.Message);
            }

            this.logger.LogInformation($"{nameof(this.FilterResults)} generated the model and ready to pass to the view");

            return await Results(model).ConfigureAwait(true);
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/searchcourse")]
        public async Task<IActionResult> SearchCourse(string searchTerm)
        {
            this.logger.LogInformation($"{nameof(this.SearchCourse)} has been called");

            var model = new BodyViewModel();
            var courseSearchFilters = new CourseSearchFilters();
            courseSearchFilters.CourseType = CourseType.All;
            courseSearchFilters.CourseHours = CourseHours.All;
            courseSearchFilters.StartDate = StartDate.Anytime;
            courseSearchFilters.SearchTerm = string.IsNullOrEmpty(searchTerm) ? string.Empty : searchTerm;

            model.SideBar = this.GetSideBarViewModel();
            model.OrderByOptions = ListFilters.GetOrderByOptions();
            model.CurrentSearchTerm = searchTerm;
            model.RequestPage = 1;

            try
            {
                model.Results = await findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.Relevance, 1);
            }
            catch (Exception ex) {
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

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.selectedIds.Any())
            {
                model.SideBar.CourseType = this.CheckCheckboxState(model.SideBar.CourseType, sideBarViewModel.CourseType);
                sideBarViewModel.CourseType.selectedIds = model.SideBar.CourseType.selectedIds;
            }

            if (model.SideBar.CourseHours != null && model.SideBar.CourseHours.selectedIds.Any())
            {
                model.SideBar.CourseHours = this.CheckCheckboxState(model.SideBar.CourseHours, sideBarViewModel.CourseHours);
                sideBarViewModel.CourseHours.selectedIds = model.SideBar.CourseHours.selectedIds;
            }

            if (model.SideBar.CourseStudyTime != null && model.SideBar.CourseStudyTime.selectedIds.Any())
            {
                model.SideBar.CourseStudyTime = this.CheckCheckboxState(model.SideBar.CourseStudyTime, sideBarViewModel.CourseStudyTime);
                sideBarViewModel.CourseStudyTime.selectedIds = model.SideBar.CourseStudyTime.selectedIds;
            }

            if (model.SideBar.StartDate != null && model.SideBar.StartDate.selectedIds.Any())
            {
                model.SideBar.StartDate = this.CheckCheckboxState(model.SideBar.StartDate, sideBarViewModel.StartDate);
                sideBarViewModel.StartDate.selectedIds = model.SideBar.StartDate.selectedIds;
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

            filterModel.lstChkFilter = checkboxList;

            this.logger.LogInformation($"{nameof(this.MapFilter)} {title} list has been generated successfully");

            return filterModel;
        }

        private FiltersListViewModel CheckCheckboxState(FiltersListViewModel model, FiltersListViewModel newModel)
        {
            this.logger.LogInformation($"{nameof(this.CheckCheckboxState)} has been called");

            foreach (var item in newModel.lstChkFilter)
            {
                var exists = model.selectedIds.Contains(item.Id);
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

            model.selectedIds = list;
            return model;
        }

        private SideBarViewModel GetSideBarViewModel()
        {
            var sideBarViewModel = new SideBarViewModel();
            sideBarViewModel.CourseType = this.MapFilter("courseType", "Course type", ListFilters.GetCourseTypeList());
            sideBarViewModel.CourseHours = this.MapFilter("courseHours", "Course hours", ListFilters.GetHoursList());
            sideBarViewModel.CourseStudyTime = this.MapFilter("courseStudyTime", "Course study time", ListFilters.GetStudyTimeList());
            sideBarViewModel.StartDate = this.MapFilter("courseStartDate", "Start date", ListFilters.GetStartDateList());
            sideBarViewModel.DistanceOptions = ListFilters.GetDistanceList();

            return sideBarViewModel;
        }
    }
}