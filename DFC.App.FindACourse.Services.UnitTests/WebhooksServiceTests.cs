using AutoMapper;
using DFC.App.FindACourse.Data.Enums;
using DFC.App.FindACourse.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.Services.UnitTests
{
    [Trait("Category", "Webhook Service Unit Tests")]
    public class WebhooksServiceTests
    {
        private readonly ILogger<WebhooksService> fakeLogger = A.Fake<ILogger<WebhooksService>>();
        private readonly IMapper fakeMapper = A.Fake<IMapper>();
        private readonly ICmsApiService fakeCmsApiService = A.Fake<ICmsApiService>();
        private readonly IDocumentService<StaticContentItemModel> fakeConfigurationSetDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();

        public static IEnumerable<object[]> TestDeleteContentItemData => new List<object[]>
        {
            new object[] { true, HttpStatusCode.OK },
            new object[] { false, HttpStatusCode.NoContent },
        };

        public static IEnumerable<object[]> TestValidationData => new List<object[]>
        {
            new object[] { BuildValidStaticContentItemModel(), true },
            new object[] { A.Fake<StaticContentItemModel>(), false },
        };

        [Fact]
        public async Task ProcessMessageAsyncDeleteReturnsSuccess()
        {
            //Arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);
            var dummyStaticContentItemApiDataModel = A.Dummy<StaticContentItemApiDataModel>();
            var validStaticContentItemModel = BuildValidStaticContentItemModel();

            A.CallTo(() => fakeConfigurationSetDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(true);

            //Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.Delete, Guid.NewGuid(), Guid.NewGuid(), "https://somewhere.com").ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeConfigurationSetDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeConfigurationSetDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessMessageAsyncCreateOrUpdateReturnsSuccess()
        {
            //Arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);
            var dummyStaticContentItemApiDataModel = A.Dummy<StaticContentItemApiDataModel>();
            var validStaticContentItemModel = BuildValidStaticContentItemModel();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).Returns(dummyStaticContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).Returns(validStaticContentItemModel);
            A.CallTo(() => fakeConfigurationSetDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).Returns(expectedResult);

            //Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), Guid.NewGuid(), "https://somewhere.com").ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConfigurationSetDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConfigurationSetDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessMessageAsyncCreateOrUpdateThrowsInvalidDataException()
        {
            //Arrange
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);

            //Act
            await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ProcessMessageAsync(WebhookCacheOperation.CreateOrUpdate, Guid.NewGuid(), Guid.NewGuid(), "http//somewhere.com").ConfigureAwait(false)).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeConfigurationSetDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeConfigurationSetDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task ProcessMessageAsyncNoneReturnsBadRequest()
        {
            //Arrange
            const HttpStatusCode expectedResult = HttpStatusCode.BadRequest;
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);

            //Act
            var result = await service.ProcessMessageAsync(WebhookCacheOperation.None, Guid.NewGuid(), Guid.NewGuid(), "https://somewhere.com").ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeConfigurationSetDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeConfigurationSetDocumentService.DeleteAsync(A<Guid>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessContentItemAsyncReturnsSuccess()
        {
            //Arrange
            const HttpStatusCode expectedResult = HttpStatusCode.OK;
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);
            var dummyStaticContentItemApiDataModel = A.Dummy<StaticContentItemApiDataModel>();
            var validStaticContentItemModel = BuildValidStaticContentItemModel();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).Returns(dummyStaticContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).Returns(validStaticContentItemModel);
            A.CallTo(() => fakeConfigurationSetDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).Returns(expectedResult);

            //Act
            var result = await service.ProcessContentItemAsync(new Uri("https://somewhere.com", UriKind.Absolute)).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConfigurationSetDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessContentItemAsyncReturnsNoContent()
        {
            //Arrange
            const HttpStatusCode expectedResult = HttpStatusCode.NoContent;
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);
            var dummyStaticContentItemApiDataModel = A.Dummy<StaticContentItemApiDataModel>();
            StaticContentItemModel nullStaticContentItemModel = null;

            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).Returns(dummyStaticContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).Returns(nullStaticContentItemModel);

            //Act
            var result = await service.ProcessContentItemAsync(new Uri("https://somewhere.com", UriKind.Absolute)).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConfigurationSetDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ProcessContentItemAsyncReturnsBadRequest()
        {
            //Arrange
            const HttpStatusCode expectedResult = HttpStatusCode.BadRequest;
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);
            var dummyStaticContentItemApiDataModel = A.Dummy<StaticContentItemApiDataModel>();
            var dummyStaticContentItemModel = A.Dummy<StaticContentItemModel>();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).Returns(dummyStaticContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).Returns(dummyStaticContentItemModel);

            //Act
            var result = await service.ProcessContentItemAsync(new Uri("https://somewhere.com", UriKind.Absolute)).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<Uri>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeConfigurationSetDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustNotHaveHappened();

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [MemberData(nameof(TestDeleteContentItemData))]
        public async Task DeleteContentItemAsyncReturnsSuccess(bool deletionResult, HttpStatusCode expectedResult)
        {
            //Arrange
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);

            A.CallTo(() => fakeConfigurationSetDocumentService.DeleteAsync(A<Guid>.Ignored)).Returns(deletionResult);

            //Act
            var result = await service.DeleteContentItemAsync(Guid.NewGuid()).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeConfigurationSetDocumentService.DeleteAsync(A<Guid>.Ignored)).MustHaveHappenedOnceExactly();

            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [MemberData(nameof(TestValidationData))]
        public void TryValidateModelForValidAndInvalid(StaticContentItemModel model, bool expectedResult)
        {
            //Arrange
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);

            //Act
            var result = service.TryValidateModel(model);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void TryValidateModelReturnsArgumentNullException()
        {
            //Arrange
            var service = new WebhooksService(fakeLogger, fakeMapper, fakeCmsApiService, fakeConfigurationSetDocumentService);
            StaticContentItemModel model = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.TryValidateModel(model));
        }

        private static StaticContentItemModel BuildValidStaticContentItemModel()
        {
            return new StaticContentItemModel
            {
                Id = Guid.NewGuid(),
                Title = "A title",
                Url = new Uri("https://somewhere.com", UriKind.Absolute),
                Content = "<p>Some content</p>",
            };
        }
    }
}
