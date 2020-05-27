using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Data.Helpers;
using DFC.App.FindACourse.Extensions;
using DFC.App.FindACourse.Services;
using DFC.App.FindACourse.ViewModels;
using DFC.CompositeInterfaceModels.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using GdsCheckboxList.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
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
        private readonly ILogService logService;
        private readonly IFindACourseService findACourseService;

        public CourseController(ILogService logService, IFindACourseService findACourseService)
        {
            this.logService = logService;
            this.findACourseService = findACourseService;
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
            this.logService.LogInformation($"{nameof(this.Document)} has been called");

            var model = new DocumentViewModel
            {
                Title = "Find a Course",
                BreadcrumbTitle = "Breadcrumb 1",
                Keywords = "fac",
                CanonicalName = "find-a-course",
                Content = new HtmlString("Document Content"),
                IncludeInSitemap = true,
            };

            this.logService.LogInformation($"{nameof(this.Document)} generated the model and ready to pass to the view");

            return this.View(model);
        }

        [HttpGet]
        public IActionResult Head()
        {
            this.logService.LogInformation($"{nameof(this.Head)} has been called");

            var model = new HeadViewModel { Title = "Find a Course", Description = "FAC", Keywords = "fac", CanonicalUrl = "find-a-course" };

            this.logService.LogInformation($"{nameof(this.Head)} generated the model and ready to pass to the view");

            return View(model);
        }

        [HttpGet]
        public IActionResult Breadcrumb()
        {
            this.logService.LogInformation($"{nameof(this.Breadcrumb)} has been called");

            var model = new BreadcrumbViewModel
            {
                Paths = new List<BreadcrumbPathViewModel>
                {
                    new BreadcrumbPathViewModel { Route = "/", Title = "Home", AddHyperlink = true },
                    new BreadcrumbPathViewModel { Route = "/find-a-course", Title = "Find a Course", AddHyperlink = true },
                },
            };

            this.logService.LogInformation($"{nameof(this.Breadcrumb)} generated the model and ready to pass to the view");
            return View(model);
        }

        [HttpGet]
        [Route("find-a-course/{controller}/bodytop/{**data}")]
        public IActionResult BodyTop()
        {
            this.logService.LogInformation($"{nameof(this.BodyTop)} has been called");

            return View();
        }

        [HttpGet]
        [Route("find-a-course/{controller}/body/{**data}")]
        public async Task<IActionResult> Body()
        {
            this.logService.LogInformation($"{nameof(this.Body)} has been called");

            var model = new BodyViewModel();

            model = new BodyViewModel { Content = new HtmlString("Find a course: Body element") };
            model.SideBar = this.GetSideBarViewModel();
            model.OrderByOptions = ListFilters.GetOrderByOptions();

            this.logService.LogInformation($"{nameof(this.Body)} generated the model and ready to pass to the view");

            return await this.SearchCourse(string.Empty).ConfigureAwait(true);
        }

        [HttpGet]
        [Route("find-a-course/{controller}/bodyfooter/{**data}")]
        public IActionResult BodyFooter()
        {
            this.logService.LogInformation($"{nameof(this.BodyFooter)} has been called");

            return this.NoContent();
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/page")]
        public async Task<IActionResult> Page(string searchTerm, string town, string distance, string courseType, string courseHours, string studyTime, string startDate, int page)
        {
            this.logService.LogInformation($"{nameof(this.Page)} has been called");

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
                    StartDateValue = startDate,
                },
                RequestPage = page,
            };

            this.logService.LogInformation($"{nameof(this.Page)} generated the model and ready to pass to the view");
            return await this.FilterResults(model).ConfigureAwait(false);
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/filterresults")]
        public async Task<IActionResult> FilterResults(BodyViewModel model)
        {
            this.logService.LogInformation($"{nameof(this.FilterResults)} has been called");

            var courseTypeList = new List<CourseType>();
            var courseHoursList = new List<CourseHours>();
            var courseStudyTimeList = new List<Fac.AttendancePattern>();
            var sortedByCriteria = CourseSearchOrderBy.StartDate;
            var selectedStartDateValue = StartDate.Anytime;

            float selectedDistanceValue = 10;

            if (model.SelectedDistanceValue != null)
            {
                var resultString = Regex.Match(model.SelectedDistanceValue, @"\d+").Value;
                _ = float.TryParse(resultString, out selectedDistanceValue);
            }

            if (model.SideBar.CourseType != null && model.SideBar.CourseType.SelectedIds.Any())
            {
                courseTypeList = this.ConvertToEnumList<CourseType>(model.SideBar.CourseType.SelectedIds);
            }

            if (model.SideBar.CourseHours != null && model.SideBar.CourseHours.SelectedIds.Any())
            {
                courseHoursList = this.ConvertToEnumList<CourseHours>(model.SideBar.CourseHours.SelectedIds);
            }

            if (model.SideBar.CourseStudyTime != null && model.SideBar.CourseStudyTime.SelectedIds.Any())
            {
                courseStudyTimeList = this.ConvertToEnumList<Fac.AttendancePattern>(model.SideBar.CourseStudyTime.SelectedIds);
            }

            _ = Enum.TryParse(model.SelectedOrderByValue, out sortedByCriteria);

            var courseSearchFilters = new CourseSearchFilters
            {
                SearchTerm = model.CurrentSearchTerm,
                CourseType = courseTypeList,
                CourseHours = courseHoursList,
                StartDate = selectedStartDateValue,
                CourseStudyTime = courseStudyTimeList,
                Distance = selectedDistanceValue,
            };

            switch (model.SideBar.StartDateValue)
            {
                case "Next 3 months":
                    courseSearchFilters.StartDateFrom = DateTime.Today;
                    courseSearchFilters.StartDateTo = DateTime.Today.AddMonths(3);
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
                    if (this.IsPostcode(model.SideBar.TownOrPostcode))
                    {
                        courseSearchFilters.PostCode = this.NormalizePostcode(model.SideBar.TownOrPostcode);
                        courseSearchFilters.Distance = selectedDistanceValue;
                        courseSearchFilters.DistanceSpecified = true;
                    }
                    else
                    {
                        courseSearchFilters.Town = model.SideBar.TownOrPostcode;
                    }
                }

                // Enter filters criteria here
            model.RequestPage = (model.RequestPage > 1) ? model.RequestPage : 1;

            try
                {
                    model.Results = await this.findACourseService.GetFilteredData(courseSearchFilters, sortedByCriteria, model.RequestPage).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.logService.LogError($"{nameof(this.FilterResults)} threw an exception" + ex.Message);
                }

            this.logService.LogInformation($"{nameof(this.FilterResults)} generated the model and ready to pass to the view");

            return this.Results(model);
        }

        [HttpGet]
        [Route("find-a-course/course/body/course/searchcourse")]
        public async Task<IActionResult> SearchCourse(string searchTerm)
        {
            this.logService.LogInformation($"{nameof(this.SearchCourse)} has been called");

            var model = new BodyViewModel();
            var courseSearchFilters = new CourseSearchFilters
            {
                CourseType = new List<CourseType> { CourseType.All },
                CourseHours = new List<CourseHours> { CourseHours.All },
                StartDate = StartDate.Anytime,
                CourseStudyTime = new List<Fac.AttendancePattern> { Fac.AttendancePattern.Undefined },
                SearchTerm = string.IsNullOrEmpty(searchTerm) ? string.Empty : searchTerm,
            };

            model.SideBar = this.GetSideBarViewModel();
            model.OrderByOptions = ListFilters.GetOrderByOptions();
            model.CurrentSearchTerm = searchTerm;
            model.RequestPage = 1;

            try
            {
                model.Results = await this.findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.StartDate, 1).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                this.logService.LogError($"{nameof(this.SearchCourse)} threw an exception" + ex.Message);
            }

            this.logService.LogInformation($"{nameof(this.SearchCourse)} generated the model and ready to pass to the view");

            return this.Results(model);
        }

        [HttpGet]
        public IActionResult Results(BodyViewModel model)
        {
            this.logService.LogInformation($"{nameof(this.Results)} has been called");

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
            sideBarViewModel.StartDateValue = model.SideBar.StartDateValue;
            sideBarViewModel.DistanceValue = model.SelectedDistanceValue;

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

            this.logService.LogInformation($"{nameof(this.Results)} generated the model and ready to pass to the view");

            return View("Body", model);
        }

        private FiltersListViewModel MapFilter(string text, string title, List<Filter> lstFilter)
        {
            this.logService.LogInformation($"{nameof(this.MapFilter)} has been called for {title}");

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

            this.logService.LogInformation($"{nameof(this.MapFilter)} {title} list has been generated successfully");

            return filterModel;
        }

        private FiltersListViewModel CheckCheckboxState(FiltersListViewModel model, FiltersListViewModel newModel)
        {
            this.logService.LogInformation($"{nameof(this.CheckCheckboxState)} has been called");

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
                StartDateOptions = ListFilters.GetStartDateList(),
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

        private bool IsPostcode(string townOrPostcode)
        {
            var postcodeRegex = new Regex(@"^([A-Z][A-HJ-Y]?\d[A-Z\d]? ?\d[A-Z]{2}|GIR ?0A{2})$");

            if (postcodeRegex.IsMatch(townOrPostcode.ToUpper()))
            {
                return true;
            }

            return false;
        }

        private string NormalizePostcode(string postcode)
        {
            //removes end and start spaces 
            postcode = postcode.Trim();
            //removes in middle spaces 
            postcode = postcode.Replace(" ", "");

            return postcode.Insert(postcode.Length - 3, " ");
        }
    }
}