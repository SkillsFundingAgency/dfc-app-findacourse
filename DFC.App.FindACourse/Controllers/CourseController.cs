using System;
using DFC.App.FindACourse.Extensions;
using DFC.App.FindACourse.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.App.FindACourse.Services;
using Microsoft.Extensions.Logging;
using DFC.App.FindACourse.Data.Domain;
using DFC.FindACourseClient;
using CheckBoxList.Core.Models;
using DFC.App.FindACourse.Data.Helpers;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

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
        public async Task<IActionResult> Document(string article)
        {
            var model = new DocumentViewModel
            {
                Title = "Find a Course",
                BreadcrumbTitle = "Breadcrumb 1",
                Keywords = "fac",
                CanonicalName = "find-a-course",
                Content = new HtmlString("Document Content"),
                IncludeInSitemap = true,
            };

            return View(model);
        }

        [HttpGet]
        [Route("course/head")]
        public async Task<IActionResult> Head()
        {
            var model = new HeadViewModel { Title = "Find a Course", Description = "FAC", Keywords = "fac", CanonicalUrl = "find-a-course" };

            return View(model);
        }

        [HttpGet]
        [Route("course/herobanner")]
        public async Task<IActionResult> HeroBanner()
        {
            return View();
        }

        public async Task<IActionResult> Breadcrumb()
        {
            var model = new BreadcrumbViewModel
            {
                Paths = new List<BreadcrumbPathViewModel>
                {
                    new BreadcrumbPathViewModel { Route = "/", Title = "Home", AddHyperlink = true },
                    new BreadcrumbPathViewModel { Route = "/find-a-course", Title = "Find a Course", AddHyperlink = true },
                },
            };

            return View(model);
        }

        [HttpGet]
        [Route("course/bodytop")]
        public async Task<IActionResult> BodyTop()
        {
            return View();
        }

        [HttpGet]
        [Route("course/body")]
        public async Task<IActionResult> Body()
        {
            logger.LogInformation($"{nameof(this.Body)} has been called");
            var model = new BodyViewModel();

            model = new BodyViewModel { Content = new HtmlString("Find a course: Body element") };
            var sideBarViewModel = new SideBarViewModel();
            sideBarViewModel.CourseType = MapFilter("courseType", "Course type", ListFilters.GetCourseTypeList());
            sideBarViewModel.CourseHours = MapFilter("courseHours", "Course hours", ListFilters.GetHoursList());
            sideBarViewModel.CourseStudyTime = MapFilter("courseStudyTime", "Course study time", ListFilters.GetStudyTimeList());
            sideBarViewModel.StartDate = MapFilter("courseStartDate", "Start date", ListFilters.GetStartDateList());
            sideBarViewModel.DistanceOptions = ListFilters.GetDistanceList();
            model.SideBar = sideBarViewModel;
            model.OrderByOptions = ListFilters.GetOrderByOptions();

            logger.LogInformation($"{nameof(this.Body)} generated the model and ready to pass to the view");

            return View(model);
        }

        [HttpGet]
        [Route("course/bodyfooter")]
        public IActionResult BodyFooter(string article)
        {
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> FilterResults(BodyViewModel model)
        {
            CourseType courseTypeCriteria = CourseType.All;
            CourseHours courseHoursCriteria = CourseHours.All;
            StartDate courseStartDateCriteria = StartDate.Anytime;
            float selectedDistanceValue = 10;

            if (!ModelState.IsValid) {
                // handle error
            }

            if (model.SideBar.DistanceOptions != null)
            {
                float.TryParse(model.SideBar.DistanceValue, out selectedDistanceValue);
                //Enum.TryParse(model, out CourseSearchOrderBy sortedByCriteria);
            }

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.selectedIds.Any())
            {
                Enum.TryParse(model.SideBar.CourseType.selectedIds[0], out courseTypeCriteria);
            }

            if (model.SideBar.CourseHours != null && model.SideBar.CourseHours.selectedIds.Any())
            {

                Enum.TryParse(model.SideBar.CourseHours.selectedIds[0], out courseHoursCriteria);
             }


            if (model.SideBar.StartDate != null && model.SideBar.StartDate.selectedIds.Any())
            {
                Enum.TryParse(model.SideBar.StartDate.selectedIds[0], out courseStartDateCriteria);
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
            model.Results = await findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.Relevance, 1);

            return await Results(model).ConfigureAwait(true);
        }


        [HttpPost]
        public async Task<IActionResult> SearchCourse(string searchTerm)
        {
            var model = new BodyViewModel();
            var courseSearchFilters = new CourseSearchFilters();
            courseSearchFilters.CourseType = CourseType.All;
            courseSearchFilters.CourseHours = CourseHours.All;
            courseSearchFilters.StartDate = StartDate.Anytime;
            courseSearchFilters.SearchTerm = searchTerm;

            var sideBarViewModel = new SideBarViewModel();
            sideBarViewModel.CourseType = MapFilter("courseType", "Course type", ListFilters.GetCourseTypeList());
            sideBarViewModel.CourseHours = MapFilter("courseHours", "Course hours", ListFilters.GetHoursList());
            sideBarViewModel.CourseStudyTime = MapFilter("courseStudyTime", "Course study time", ListFilters.GetStudyTimeList());
            sideBarViewModel.StartDate = MapFilter("courseStartDate", "Start date", ListFilters.GetStartDateList());
            sideBarViewModel.DistanceOptions = ListFilters.GetDistanceList();
            model.SideBar = sideBarViewModel;
            model.OrderByOptions = ListFilters.GetOrderByOptions();
            model.CurrentSearchTerm = searchTerm;

            model.Results = await findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.Relevance, 1);

            return await Results(model).ConfigureAwait(true);
        }

        [HttpPost]
        public async Task<IActionResult> Results(BodyViewModel model)
        {
            logger.LogInformation($"{nameof(this.Body)} has been called");

            var sideBarViewModel = new SideBarViewModel();
            sideBarViewModel.CourseType = MapFilter("courseType", "Course type", ListFilters.GetCourseTypeList());
            sideBarViewModel.CourseHours = MapFilter("courseHours", "Course hours", ListFilters.GetHoursList());
            sideBarViewModel.CourseStudyTime = MapFilter("courseStudyTime", "Course study time", ListFilters.GetStudyTimeList());
            sideBarViewModel.StartDate = MapFilter("courseStartDate", "Start date", ListFilters.GetStartDateList());
            sideBarViewModel.DistanceOptions = ListFilters.GetDistanceList();
            foreach (var item in sideBarViewModel.DistanceOptions)
            {
                if (item.Value == model.SideBar.DistanceValue)
                {
                    item.Selected = true;
                    break;
                }
            }

            sideBarViewModel.TownOrPostcode = model.SideBar.TownOrPostcode;
            if (model.SideBar.CourseType != null && model.SideBar.CourseType.selectedIds.Any())
            {
                model.SideBar.CourseType = CheckCheckboxState(model.SideBar.CourseType, sideBarViewModel.CourseType);
            }

            if (model.SideBar.CourseHours != null && model.SideBar.CourseHours.selectedIds.Any())
            {
                model.SideBar.CourseHours = CheckCheckboxState(model.SideBar.CourseHours, sideBarViewModel.CourseHours);
            }

            if (model.SideBar.CourseStudyTime != null && model.SideBar.CourseStudyTime.selectedIds.Any())
            {
                model.SideBar.CourseStudyTime = CheckCheckboxState(model.SideBar.CourseStudyTime, sideBarViewModel.CourseStudyTime);
            }

            if (model.SideBar.StartDate != null && model.SideBar.StartDate.selectedIds.Any())
            {
                model.SideBar.StartDate = CheckCheckboxState(model.SideBar.StartDate, sideBarViewModel.StartDate);
            }

            model.SideBar = sideBarViewModel;
            model.OrderByOptions = ListFilters.GetOrderByOptions();

            return View("Body", model);
        }

        [HttpPost]
        [Route("pages/sort-results")]
        public async Task<IActionResult> SortBy(string criteria)
        {
            if (criteria == null)
            {
                // return error message
            }

            Enum.TryParse("REPLACE POST VARIABLE", out CourseSearchOrderBy sortedByCriteria);

            var sortedResult = findACourseService.GetSortedData(sortedByCriteria, 25);

            return null;
        }

        private FiltersListViewModel MapFilter(string text, string title, List<Filter> lstFilter)
        {
            this.logger.LogInformation($"{nameof(this.MapFilter)} has been called for {title}");

            var filterModel = new FiltersListViewModel
            {
                FilterText = text,
                FilterTitle = title
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
    }
}