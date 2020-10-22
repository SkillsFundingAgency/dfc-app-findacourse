using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Data.Contracts;
using DFC.App.FindACourse.Services;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Mime;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    public class BaseController
    {
        protected const string DefaultHelpArticleName = "help";

        public BaseController()
        {
            FakeLogService = A.Fake<ILogService>();
            FakeFindACoursesService = A.Fake<IFindACourseService>();
            FakeSharedContentService = A.Fake<ISharedContentService>();
        }

        public static IEnumerable<object[]> HtmlMediaTypes => new List<object[]>
        {
            new string[] { "*/*" },
            new string[] { MediaTypeNames.Text.Html },
        };

        public static IEnumerable<object[]> InvalidMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Text.Plain },
        };

        public static IEnumerable<object[]> JsonMediaTypes => new List<object[]>
        {
            new string[] { MediaTypeNames.Application.Json },
        };

        protected ILogService FakeLogService { get; }

       // protected ILogger<DetailsController> DetailsLogger { get; }

        protected IFindACourseService FakeFindACoursesService { get; }

        protected AutoMapper.IMapper FakeMapper { get; }

        protected ISharedContentService FakeSharedContentService { get; set; }

        protected CourseController BuildCourseController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new CourseController(FakeLogService, FakeFindACoursesService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }

        protected DetailsController BuildDetailsController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new DetailsController(FakeLogService, FakeFindACoursesService, FakeSharedContentService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
            };

            return controller;
        }
    }
}
