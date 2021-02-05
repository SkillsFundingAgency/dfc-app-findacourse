using AutoMapper;
using DFC.App.FindACourse.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DFC.App.FindACourse.Services.UnitTests
{
    [Trait("Category", "Static content reload Service Unit Tests")]
    public class StaticContentReloadServiceTests
    {
        private readonly ILogger<StaticContentReloadService> fakeLogger = A.Fake<ILogger<StaticContentReloadService>>();
        private readonly IMapper fakeMapper = A.Fake<IMapper>();
        private readonly IDocumentService<StaticContentItemModel> fakeStaticContentDocumentService = A.Fake<IDocumentService<StaticContentItemModel>>();
        private readonly ICmsApiService fakeCmsApiService = A.Fake<ICmsApiService>();
        private readonly CmsApiClientOptions fakeCmsApiClientOptions = A.Fake<CmsApiClientOptions>();

        public StaticContentReloadServiceTests()
        {
            fakeCmsApiClientOptions.ContentIds = $"{Guid.NewGuid()},{Guid.NewGuid()}";
        }

        public static IEnumerable<object[]> TestValidationData => new List<object[]>
        {
            new object[] { BuildValidStaticContentItemModel(), true },
            new object[] { A.Fake<StaticContentItemModel>(), false },
        };

        [Fact]
        public async Task ReloadIsCancelled()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var service = new StaticContentReloadService(fakeLogger, fakeMapper, fakeStaticContentDocumentService, fakeCmsApiService, fakeCmsApiClientOptions);

            //Act
            await service.Reload(cancellationToken).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeStaticContentDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustNotHaveHappened();

            Assert.True(true);
        }

        [Fact]
        public async Task ReloadIsSuccessful()
        {
            //Arrange
            var cancellationToken = new CancellationToken(false);
            var service = new StaticContentReloadService(fakeLogger, fakeMapper, fakeStaticContentDocumentService, fakeCmsApiService, fakeCmsApiClientOptions);
            var dummyStaticContentItemApiDataModel = A.Dummy<StaticContentItemApiDataModel>();
            var staticContentItemModel = BuildValidStaticContentItemModel();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(dummyStaticContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).Returns(staticContentItemModel);
            A.CallTo(() => fakeStaticContentDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).Returns(HttpStatusCode.OK);

            //Act
            await service.Reload(cancellationToken).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => fakeStaticContentDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustHaveHappenedOnceOrMore();

            Assert.True(true);
        }

        [Fact]
        public async Task ReloadCatchesAndThrowsException()
        {
            //Arrange
            var cancellationToken = new CancellationToken(false);
            var service = new StaticContentReloadService(fakeLogger, fakeMapper, fakeStaticContentDocumentService, fakeCmsApiService, fakeCmsApiClientOptions);

            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Throws<Exception>();

            //Act
            await Assert.ThrowsAsync<Exception>(async () => await service.Reload(cancellationToken).ConfigureAwait(false)).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeStaticContentDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustNotHaveHappened();

            Assert.True(true);
        }

        [Fact]
        public async Task ReloadSharedContentIsCancelled()
        {
            //Arrange
            var cancellationToken = new CancellationToken(true);
            var service = new StaticContentReloadService(fakeLogger, fakeMapper, fakeStaticContentDocumentService, fakeCmsApiService, fakeCmsApiClientOptions);

            //Act
            await service.ReloadSharedContent(cancellationToken).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => fakeStaticContentDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustNotHaveHappened();

            Assert.True(true);
        }

        [Fact]
        public async Task ReloadSharedContentWithValidationErrors()
        {
            //Arrange
            var cancellationToken = new CancellationToken(false);
            var service = new StaticContentReloadService(fakeLogger, fakeMapper, fakeStaticContentDocumentService, fakeCmsApiService, fakeCmsApiClientOptions);
            StaticContentItemApiDataModel staticContentItemApiDataModel = null;
            var dummyStaticContentItemModel = A.Dummy<StaticContentItemModel>();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(staticContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).Returns(dummyStaticContentItemModel);

            //Act
            await service.ReloadSharedContent(cancellationToken).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => fakeStaticContentDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustNotHaveHappened();

            Assert.True(true);
        }

        [Fact]
        public async Task ReloadSharedContentIsSuccessful()
        {
            //Arrange
            var cancellationToken = new CancellationToken(false);
            var service = new StaticContentReloadService(fakeLogger, fakeMapper, fakeStaticContentDocumentService, fakeCmsApiService, fakeCmsApiClientOptions);
            var dummyStaticContentItemApiDataModel = A.Dummy<StaticContentItemApiDataModel>();
            var staticContentItemModel = BuildValidStaticContentItemModel();

            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).Returns(dummyStaticContentItemApiDataModel);
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).Returns(staticContentItemModel);
            A.CallTo(() => fakeStaticContentDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).Returns(HttpStatusCode.OK);

            //Act
            await service.ReloadSharedContent(cancellationToken).ConfigureAwait(false);

            //Assert
            A.CallTo(() => fakeCmsApiService.GetItemAsync<StaticContentItemApiDataModel>(A<string>.Ignored, A<Guid>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => fakeMapper.Map<StaticContentItemModel>(A<StaticContentItemApiDataModel>.Ignored)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => fakeStaticContentDocumentService.UpsertAsync(A<StaticContentItemModel>.Ignored)).MustHaveHappenedOnceOrMore();

            Assert.True(true);
        }

        [Theory]
        [MemberData(nameof(TestValidationData))]
        public void TryValidateModelForValidAndInvalid(StaticContentItemModel model, bool expectedResult)
        {
            //Arrange
            var service = new StaticContentReloadService(fakeLogger, fakeMapper, fakeStaticContentDocumentService, fakeCmsApiService, fakeCmsApiClientOptions);

            //Act
            var result = service.TryValidateModel(model);

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void TryValidateModelReturnsArgumentNullException()
        {
            //Arrange
            var service = new StaticContentReloadService(fakeLogger, fakeMapper, fakeStaticContentDocumentService, fakeCmsApiService, fakeCmsApiClientOptions);
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
