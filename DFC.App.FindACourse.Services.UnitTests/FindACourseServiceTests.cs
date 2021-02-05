using DFC.App.FindACourse.Repository;
using DFC.CompositeInterfaceModels.FindACourseClient;
using FakeItEasy;
using System.Collections.Generic;
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
            courseProperties.Filters.CourseType = new List<CourseType> { CourseType.All };
            courseProperties.Filters.CourseHours = new List<CourseHours> { CourseHours.All };

            var courseSearchFilters = new CourseSearchFilters
            {
                SearchTerm = "Maths",
                CourseType = new List<CourseType> { CourseType.All },
                CourseHours = new List<CourseHours> { CourseHours.All },
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
        public void CheckEnumOfCourseTypeIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<CourseType> { CourseType.All, CourseType.ClassroomBased, CourseType.ClassroomBased, CourseType.WorkBased, };

            A.CallTo(() => repository.GetFilter<CourseType>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<CourseType>();

            //Assert
            Assert.Equal(4, result.Count);
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
    }
}