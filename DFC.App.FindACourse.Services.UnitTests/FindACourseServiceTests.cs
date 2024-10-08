using DFC.App.FindACourse.Repository;
using DFC.CompositeInterfaceModels.FindACourseClient;
using FakeItEasy;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Fac = DFC.FindACourseClient;

namespace DFC.App.FindACourse.Services.UnitTests
{
    [Trait("Category", "Find a course Service Unit Tests")]
    public class FindACourseServiceTests
    {
        [Fact]
        public void PingAsyncReturnsSuccess()
        {
            //Arrange
            const bool expectedResult = true;
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);

            A.CallTo(() => repository.PingAsync()).Returns(expectedResult);

            //Act
            var result = findACourseService.PingAsync();

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void CheckIfFilteredDataReturnsData()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var courseProperties = new CourseSearchProperties
            {
                Page = 1,
            };
            courseProperties.Filters.SearchTerm = "Maths";
            courseProperties.Filters.LearningMethod = new List<LearningMethod>() { LearningMethod.All };
            courseProperties.Filters.CourseType = new List<CourseType>();
            courseProperties.Filters.CourseHours = new List<CourseHours> { CourseHours.All };

            var courseSearchFilters = new CourseSearchFilters
            {
                SearchTerm = "Maths",
                CourseType = new List<CourseType>(),
                CourseHours = new List<CourseHours> { CourseHours.All },
                LearningMethod = new List<LearningMethod>() { LearningMethod.All },
            };

            var returnedCourseData = new CourseSearchResult
            {
                Courses = new List<Course>
                {
                    new Course { Title = "Maths", CourseId = "1", AttendancePattern = "Online", },
                },
            };

            var findACourseService = new FindACourseService(repository);

            A.CallTo(() => repository.RetrieveData(A<CourseSearchProperties>.Ignored)).Returns(returnedCourseData);

            //Act
            var result = findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.Relevance, 1).Result;

            //Assert
            A.CallTo(() => repository.RetrieveData(A<CourseSearchProperties>.Ignored)).MustHaveHappenedOnceExactly();
            Assert.Equal(returnedCourseData, result);
        }

        [Fact]
        public void CheckEnumOfLearningMethodIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<LearningMethod> { LearningMethod.All, LearningMethod.ClassroomBased, LearningMethod.ClassroomBased, LearningMethod.WorkBased, };

            A.CallTo(() => repository.GetFilter<LearningMethod>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<LearningMethod>();

            //Assert
            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void CheckEnumOfCourseTypeIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<CourseType> { CourseType.Multiply, CourseType.EssentialSkills, CourseType.SkillsBootcamp, CourseType.HTQs, CourseType.FreeCoursesForJobs, CourseType.TLevels };

            A.CallTo(() => repository.GetFilter<CourseType>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<CourseType>();

            //Assert
            Assert.Equal(6, result.Count);
        }

        [Fact]
        public void CheckIntegerOfSectorsIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<int> { 1, 2, 3 };

            A.CallTo(() => repository.GetFilter<int>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<int>();

            //Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void CheckEnumOfCourseHoursIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<CourseHours> { CourseHours.All, CourseHours.Flexible, CourseHours.Fulltime, CourseHours.PartTime };

            A.CallTo(() => repository.GetFilter<CourseHours>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<CourseHours>();

            //Assert
            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void CheckEnumOfCourseStudyTypeIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<Fac.StudyMode> { Fac.StudyMode.Flexible, Fac.StudyMode.FullTime, Fac.StudyMode.PartTime, Fac.StudyMode.Undefined, };

            A.CallTo(() => repository.GetFilter<Fac.StudyMode>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<Fac.StudyMode>();

            //Assert
            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void CheckEnumOfCourseStartDateIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<StartDate> { StartDate.Anytime, StartDate.FromToday, StartDate.SelectDateFrom, };

            A.CallTo(() => repository.GetFilter<StartDate>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<StartDate>();

            //Assert
            Assert.Equal(3, result.Count);
            Assert.Equal(result.GetType(), result.GetType());
        }

        [Fact]
        public void EnsureCourseDetailsReturnsData()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            const string courseId = "c0a5dfeb-f2a6-4000-8272-ec1fa78df265";
            const string runId = "6707d15a-5a19-4c18-9cc8-570573bb5d67";

            var returnedCourseDetails = new CourseDetails
            {
                Title = "Maths in a unit test",
                Description = "This is a maths in a top class description",
                EntryRequirements = "Bring yourself and a brain",
            };

            var findACourseService = new FindACourseService(repository);

            A.CallTo(() => repository.GetCourseDetails(courseId, runId)).Returns(returnedCourseDetails);

            //Act
            var result = findACourseService.GetCourseDetails(courseId, runId).Result;

            //Assert
            A.CallTo(() => repository.GetCourseDetails(courseId, runId)).MustHaveHappenedOnceExactly();
            Assert.Equal(returnedCourseDetails, result);
        }

        [Fact]
        public void GetTLevelDetailsTest()
        {
            // Set up
            const string tlevelId = "DummyTLevelId";
            const string TLevelLocationId = "DummyTLevelLocationId";

            var repository = A.Fake<IFindACourseRepository>();
            A.CallTo(() => repository.GetTLevelDetails(tlevelId)).Returns(A.Dummy<TLevelDetails>());

            var findACourseService = new FindACourseService(repository);

            //Act
            var result = findACourseService.GetTLevelDetails(tlevelId, TLevelLocationId)
                .Result;

            //Assert
            A.CallTo(() => repository.GetTLevelDetails(tlevelId)).MustHaveHappenedOnceExactly();
            A.Equals(result, A.Dummy<TLevelDetails>());
        }

        [Theory]
        [InlineData("id3", "VenueThree")]
        [InlineData("id2", "VenueTwo")]
        public void RequestedVenueIdIsFirstInTheList(string venueId, string expectedFirstVenue)
        {
            // Set up
            var repository = A.Fake<IFindACourseRepository>();

            A.CallTo(() => repository.GetTLevelDetails(A<string>.Ignored)).Returns(GetTLevelDetails());

            var findACourseService = new FindACourseService(repository);

            //Act
            var result = findACourseService.GetTLevelDetails("DummyTLevelId", venueId).Result;

            //Assert
            A.Equals(result.Venues.FirstOrDefault().VenueName, expectedFirstVenue);
        }

        private static TLevelDetails GetTLevelDetails()
        {
            var tTLevelDetails = new TLevelDetails() { Venues = new List<Venue>() };
            tTLevelDetails.Venues.Add(new Venue() { VenueName = "VenueOne", Id = "Id1" });
            tTLevelDetails.Venues.Add(new Venue() { VenueName = "VenueTwo", Id = "Id2" });
            tTLevelDetails.Venues.Add(new Venue() { VenueName = "VenueThree", Id = "Id3" });
            return tTLevelDetails;
        }
    }
}