using DFC.App.FindACourse.Controllers;
using DFC.App.FindACourse.Data.Contracts;
using DFC.App.FindACourse.Data.Enums;
using DFC.App.FindACourse.Data.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.UnitTests.Controllers
{
    [Trait("Category", "Webhooks Controller Unit Tests")]
    public class WebhooksControllerPostTests
    {
        protected const string EventTypePublished = "published";
        protected const string EventTypeDraft = "draft";
        protected const string EventTypeDraftDiscarded = "draft-discarded";
        protected const string EventTypeDeleted = "deleted";
        protected const string EventTypeUnpublished = "unpublished";

        protected const string ContentTypePages = "pages";

        //private readonly ILogger<WebhooksController> logger = A.Fake<ILogger<WebhooksController>>();
        private readonly IWebhooksService fakeWebhooksService = A.Fake<IWebhooksService>();

        public static IEnumerable<object[]> PublishedEvents => new List<object[]>
        {
            new object[] { MediaTypeNames.Application.Json, EventTypePublished },
            new object[] { MediaTypeNames.Application.Json, EventTypeDraft },
        };

        public static IEnumerable<object[]> DeletedEvents => new List<object[]>
        {
            new object[] { MediaTypeNames.Application.Json, EventTypeDraftDiscarded },
            new object[] { MediaTypeNames.Application.Json, EventTypeDeleted },
            new object[] { MediaTypeNames.Application.Json, EventTypeUnpublished },
        };

        public static IEnumerable<object[]> InvalidIdValues => new List<object[]>
        {
            new object[] { string.Empty },
            new object[] { "Not a Guid" },
        };

        private Guid ItemIdForCreate { get; } = Guid.NewGuid();

        private Guid ItemIdForUpdate { get; } = Guid.NewGuid();

        private Guid ItemIdForDelete { get; } = Guid.NewGuid();

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishCreatePostReturnsOkForCreate(string mediaTypeName, string eventType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForCreate.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.Created);

            // Act
            var result = await controller.ReceiveEvents().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishUpdatePostReturnsOk(string mediaTypeName, string eventType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForUpdate.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await controller.ReceiveEvents().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(DeletedEvents))]
        public async Task WebhooksControllerDeletePostReturnsSuccess(string mediaTypeName, string eventType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForDelete.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(expectedResponse);

            // Act
            var result = await controller.ReceiveEvents().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishCreatePostReturnsOkForAlreadyReported(string mediaTypeName, string eventType)
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForCreate.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.AlreadyReported);

            // Act
            var result = await controller.ReceiveEvents().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal((int)expectedResponse, okResult.StatusCode);

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(PublishedEvents))]
        public async Task WebhooksControllerPublishCreatePostReturnsExceptionForConflict(string mediaTypeName, string eventType)
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(eventType, new EventGridEventData { ItemId = ItemIdForCreate.ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController(mediaTypeName);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).Returns(HttpStatusCode.Conflict);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents().ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustHaveHappenedOnceExactly();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidIdValues))]
        public async Task WebhooksControllerPostReturnsErrorForInvalidEventId(string id)
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, new EventGridEventData { ItemId = Guid.NewGuid().ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            eventGridEvents.First().Id = id;
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents().ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            controller.Dispose();
        }

        [Theory]
        [MemberData(nameof(InvalidIdValues))]
        public async Task WebhooksControllerPostReturnsErrorForInvalidItemId(string id)
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, new EventGridEventData { ItemId = id, Api = "https://somewhere.com", });
            var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents().ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            controller.Dispose();
        }

        [Fact]
        public async Task WebhooksControllerPostReturnsErrorForUnknownEventType()
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent("Unknown", new EventGridEventData { ItemId = Guid.NewGuid().ToString(), Api = "https://somewhere.com", });
            var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents().ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            controller.Dispose();
        }

        [Fact]
        public async Task WebhooksControllerPostReturnsErrorForInvalidUrl()
        {
            // Arrange
            var eventGridEvents = BuildValidEventGridEvent(EventTypePublished, new EventGridEventData { Api = "http:http://badUrl" });
            var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await controller.ReceiveEvents().ConfigureAwait(false)).ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            controller.Dispose();
        }

        [Fact]
        public async Task WebhooksControllerSubscriptionValidationReturnsSuccess()
        {
            // Arrange
            const HttpStatusCode expectedResponse = HttpStatusCode.OK;
            string expectedValidationCode = Guid.NewGuid().ToString();
            var eventGridEvents = BuildValidEventGridEvent(Microsoft.Azure.EventGrid.EventTypes.EventGridSubscriptionValidationEvent, new SubscriptionValidationEventData(expectedValidationCode, "https://somewhere.com"));
            var controller = BuildWebhooksController(MediaTypeNames.Application.Json);
            controller.HttpContext.Request.Body = BuildStreamFromModel(eventGridEvents);

            // Act
            var result = await controller.ReceiveEvents().ConfigureAwait(false);

            // Assert
            A.CallTo(() => fakeWebhooksService.ProcessMessageAsync(A<WebhookCacheOperation>.Ignored, A<Guid>.Ignored, A<Guid>.Ignored, A<string>.Ignored)).MustNotHaveHappened();

            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<SubscriptionValidationResponse>(jsonResult.Value);

            Assert.Equal((int)expectedResponse, jsonResult.StatusCode);
            Assert.Equal(expectedValidationCode, response.ValidationResponse);

            controller.Dispose();
        }

        private static EventGridEvent[] BuildValidEventGridEvent<TModel>(string eventType, TModel data)
        {
            var models = new EventGridEvent[]
            {
                new EventGridEvent
                {
                    Id = Guid.NewGuid().ToString(),
                    Subject = $"{ContentTypePages}/a-canonical-name",
                    Data = data,
                    EventType = eventType,
                    EventTime = DateTime.Now,
                    DataVersion = "1.0",
                },
            };

            return models;
        }

        private static Stream BuildStreamFromModel<TModel>(TModel model)
        {
            var jsonData = JsonConvert.SerializeObject(model);
            byte[] byteArray = Encoding.ASCII.GetBytes(jsonData);
            MemoryStream stream = new MemoryStream(byteArray);

            return stream;
        }

        private WebhooksController BuildWebhooksController(string mediaTypeName)
        {
            var objectValidator = A.Fake<IObjectModelValidator>();
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers[HeaderNames.Accept] = mediaTypeName;

            var controller = new WebhooksController(logger, fakeWebhooksService)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                },
                ObjectValidator = objectValidator,
            };

            A.CallTo(() => controller.ObjectValidator.Validate(A<ActionContext>.Ignored, A<ValidationStateDictionary>.Ignored, A<string>.Ignored, A<object>.Ignored));

            return controller;
        }
    }
}