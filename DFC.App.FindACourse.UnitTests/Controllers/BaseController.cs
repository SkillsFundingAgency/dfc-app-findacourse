using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    public class BaseController
    {
        protected const string DefaultHelpArticleName = "help";

        public BaseController()
        {
            CourseLogger = A.Fake<ILogger<CourseController>>();
            DetailsLogger = A.Fake<ILogger<DetailsController>>();
            FakeFindACoursesService = A.Fake<IFindACourseService>();
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

        protected ILogger<CourseController> CourseLogger { get; }

        protected ILogger<DetailsController> DetailsLogger { get; }

        protected IFindACourseService FakeFindACoursesService { get; }

        protected AutoMapper.IMapper FakeMapper { get; }

        protected CourseController BuildCourseController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new CourseController(CourseLogger, FakeFindACoursesService)
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

            var controller = new DetailsController(DetailsLogger, FakeFindACoursesService)
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
