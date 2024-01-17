using AutoMapper;
using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Data.Domain;
using DFC.App.FindACourse.Data.Models;
using DFC.App.FindACourse.Helpers;
using DFC.App.FindACourse.Services;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using DFC.FindACourseClient;
using DFC.Logger.AppInsights.Contracts;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using DFC.Common.SharedContent.Pkg.Netcore.Interfaces;
using DFC.Common.SharedContent.Pkg.Netcore.Model.ContentItems.SharedHtml;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    public class BaseController
    {
        protected const string DefaultHelpArticleName = "help";
        protected const string TestContentId = "87dfb08e-13ec-42ff-9405-5bbde048827a";

        public BaseController()
        {
            FakeLogService = A.Fake<ILogService>();
            FakeFindACoursesService = A.Fake<IFindACourseService>();
            //FakeStaticContentDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();
            FakeSharedContentRedisInterface = A.Fake<ISharedContentRedisInterface>();
            CmsApiClientOptions = new CmsApiClientOptions() { ContentIds = TestContentId };
            FakeMapper = A.Fake<IMapper>();
            FakeViewHelper = A.Fake<IViewHelper>();
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

        protected IFindACourseService FakeFindACoursesService { get; }

        protected ILocationService FakeLocationsService { get; set; }

        protected IViewHelper FakeViewHelper { get; }

        protected IMapper FakeMapper { get; }

        // protected IDocumentService<StaticContentItemModel> FakeStaticContentDocumentService { get; set; }

        protected ISharedContentRedisInterface FakeSharedContentRedisInterface { get; set; }

        protected CmsApiClientOptions CmsApiClientOptions { get; set; }

        protected CourseController BuildCourseController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();
            var fakeTempDataProvider = A.Fake<ITempDataProvider>();
            var fakeCourseSearchClientSettings = A.Fake<CourseSearchClientSettings>();
            var fakeCourseSearchSettings = A.Fake<CourseSearchSettings>();
            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;
            httpContext.RequestServices = A.Fake<IServiceProvider>();
            FakeLocationsService = A.Fake<ILocationService>();

            var controller = new CourseController(FakeLogService, FakeFindACoursesService, FakeViewHelper, FakeLocationsService, fakeCourseSearchClientSettings, fakeCourseSearchSettings)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
                TempData = new TempDataDictionary(httpContext, fakeTempDataProvider),
            };

            return controller;
        }

        protected DetailsController BuildDetailsController(string mediaTypeName)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            //A.CallTo(() => FakeStaticContentDocumentService.GetByIdAsync(A<Guid>.Ignored, null)).Returns(new StaticContentItemModel() { Title = nameof(StaticContentItemModel.Title) });

            //A.CallTo(() => FakeSharedContentRedisInterface.GetDataAsync<SharedHtml>(A<Guid>.Ignored);

            //var controller = new DetailsController(FakeLogService, FakeFindACoursesService, FakeStaticContentDocumentService, CmsApiClientOptions, FakeMapper)
            //{
            //    ControllerContext = new ControllerContext()
            //    {
            //        HttpContext = httpContext,
            //    },
            //};
            var controller = new DetailsController(FakeLogService, FakeFindACoursesService, FakeSharedContentRedisInterface, CmsApiClientOptions, FakeMapper)
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
