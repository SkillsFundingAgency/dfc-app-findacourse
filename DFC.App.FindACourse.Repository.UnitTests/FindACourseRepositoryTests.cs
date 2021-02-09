using DFC.FindACourseClient;
using FakeItEasy;
using System.Threading.Tasks;
using Xunit;
using Cim = DFC.CompositeInterfaceModels.FindACourseClient;

namespace DFC.App.FindACourse.Repository.UnitTests
{
    public class FindACourseRepositoryTests
    {
        [Fact]
        public void FindACourseRepositoryPingReturnsSuccess()
        {
            // Arrange
            var fakeCourseSearchApiService = A.Fake<ICourseSearchApiService>();
            var repository = new FindACourseRepository(fakeCourseSearchApiService);

            // Act
            var result = repository.PingAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void FindACourseRepositoryGetFilterCourseHoursReturnsSuccess()
        {
            // Arrange
            var fakeCourseSearchApiService = A.Fake<ICourseSearchApiService>();
            var repository = new FindACourseRepository(fakeCourseSearchApiService);

            // Act
            var result = repository.GetFilter<CourseHours>();

            // Assert
            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void FindACourseRepositoryGetFilterStudyModeReturnsSuccess()
        {
            // Arrange
            var fakeCourseSearchApiService = A.Fake<ICourseSearchApiService>();
            var repository = new FindACourseRepository(fakeCourseSearchApiService);

            // Act
            var result = repository.GetFilter<StudyMode>();

            // Assert
            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void FindACourseRepositoryGetFilterStartDateReturnsSuccess()
        {
            // Arrange
            var fakeCourseSearchApiService = A.Fake<ICourseSearchApiService>();
            var repository = new FindACourseRepository(fakeCourseSearchApiService);

            // Act
            var result = repository.GetFilter<StartDate>();

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task FindACourseRepositoryRetrieveDataReturnsSuccess()
        {
            // Arrange
            var fakeCourseSearchApiService = A.Fake<ICourseSearchApiService>();
            var repository = new FindACourseRepository(fakeCourseSearchApiService);
            var dummyCourseSearchProperties = A.Dummy<Cim.CourseSearchProperties>();
            var dummyCourseSearchResult = A.Dummy<Cim.CourseSearchResult>();

            A.CallTo(() => fakeCourseSearchApiService.SearchCoursesAsync(A<Cim.CourseSearchProperties>.Ignored)).Returns(dummyCourseSearchResult);

            // Act
            var result = await repository.RetrieveData(dummyCourseSearchProperties).ConfigureAwait(false);

            // Assert
            Assert.Equal(dummyCourseSearchResult, result);
        }

        [Fact]
        public async Task FindACourseRepositoryGetCourseDetailsReturnsSuccess()
        {
            // Arrange
            var fakeCourseSearchApiService = A.Fake<ICourseSearchApiService>();
            var repository = new FindACourseRepository(fakeCourseSearchApiService);
            var dummyCourseDetails = A.Dummy<Cim.CourseDetails>();

            A.CallTo(() => fakeCourseSearchApiService.GetCompositeCourseDetailsAsync(A<string>.Ignored, A<string>.Ignored)).Returns(dummyCourseDetails);

            // Act
            var result = await repository.GetCourseDetails("one", "two").ConfigureAwait(false);

            // Assert
            Assert.Equal(dummyCourseDetails, result);
        }
    }
}
