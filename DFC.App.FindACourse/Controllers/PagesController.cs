using DFC.App.FindACourse.Extensions;
using DFC.App.FindACourse.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.App.FindACourse.Data.Enums;
using DFC.App.FindACourse.Services;
using Microsoft.Extensions.Logging;
using System;
using DFC.App.FindACourse.Data.Domain;

namespace DFC.App.FindACourse.Controllers
{
    public class PagesController : Controller
    {
        private readonly ILogger<PagesController> logger;
        private readonly IFindACourseService findACourseService;

        public PagesController(ILogger<PagesController> logger, IFindACourseService findACourseService)
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
        [Route("pages/{article}")]
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
        [Route("pages/{article}/htmlhead")]
        [Route("pages/htmlhead")]
        public async Task<IActionResult> Head(string article)
        {
            var model = new HeadViewModel { Title = "Find a Course", Description = "FAC", Keywords = "fac", CanonicalUrl = "find-a-course" };

            return View(model);
        }

        [HttpGet]
        [Route("pages/{article}/herobanner")]
        [Route("pages/herobanner")]
        public async Task<IActionResult> HeroBanner(string article)
        {
            return View();
        }

        [Route("pages/{article}/breadcrumb")]
        [Route("pages/breadcrumb")]
        public async Task<IActionResult> Breadcrumb(string article)
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
        [Route("pages/{article}/bodytop")]
        [Route("pages/bodytop")]
        public async Task<IActionResult> BodyTop(string article)
        {
            return View();
        }

        [HttpGet]
        [Route("pages/{article}/sidebarleft")]
        [Route("pages/sidebarleft")]
        public async Task<IActionResult> SideBarLeft()
        {
            var model = new SideBarViewModel();
            model.CourseFilter = MapFilter<CourseType>("course","Course type", findACourseService.GetFilterByName<CourseType>());
            model.CourseHours = MapFilter<CourseHours>("courseHours", "Course hours", findACourseService.GetFilterByName<CourseHours>());
            model.CourseStudyTime = MapFilter<CourseStudyTime>("courseStudyTime", "Course study time", findACourseService.GetFilterByName<CourseStudyTime>());
            model.StartDate = MapFilter<StartDate>("startDate", "Start date", findACourseService.GetFilterByName<StartDate>());

            return View(model);
        }

        private FiltersListViewModel MapFilter<T>(string text, string title, IList<T> lstFilter)
        {
            var filterModel = new FiltersListViewModel
            {
                FilterText = text,
                FilterTitle = title
            };

            var returnList = new List<Filter>();
            foreach (var filter in lstFilter)
            {
                returnList.Add(new Filter {Text = filter.ToString()});
            }

            filterModel.LstFilter = returnList;

            return filterModel;
        }

        [HttpGet]
        [Route("pages/{article}/contents")]
        [Route("pages/contents")]
        public async Task<IActionResult> Body(string article)
        {
            var model = new BodyViewModel { Content = new HtmlString("Find a course: Body element") };

            return View(model);
        }

        [HttpGet]
        [Route("pages/{article}/bodyfooter")]
        [Route("pages/bodyfooter")]
        public IActionResult BodyFooter(string article)
        {
            return NoContent();
        }
    }
}