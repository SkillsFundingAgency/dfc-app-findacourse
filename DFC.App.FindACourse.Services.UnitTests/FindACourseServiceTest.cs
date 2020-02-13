using DFC.App.FindACourse.Repository;
using DFC.FindACourseClient;
using FakeItEasy;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace DFC.App.FindACourse.Services.UnitTests
{
    [Trait("Category", "Find a course Service Unit Tests")]
    public class Tests
    {
        [Test]
        public void CheckIfFilteredDataReturnsData()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var courseProperties = new CourseSearchProperties();
            courseProperties.Page = 1;
            courseProperties.Filters.SearchTerm = "Maths";
            courseProperties.Filters.CourseType = CourseType.All;
            courseProperties.Filters.CourseHours = CourseHours.All;
            courseProperties.Filters.StartDate = StartDate.Anytime;

            var courseSearchFilters = new CourseSearchFilters();
            courseSearchFilters.SearchTerm = "Maths";
            courseSearchFilters.CourseType = CourseType.All;
            courseSearchFilters.CourseHours = CourseHours.All;
            courseSearchFilters.StartDate = StartDate.Anytime;

            var returnedCourseData = new CourseSearchResult();
            returnedCourseData.Courses = new List<Course>
            {
                new Course { Title= "Maths", CourseId = "1", AttendancePattern = "Online"}
            };
            
            var findACourseService = new FindACourseService(repository);

            A.CallTo(() => repository.RetrieveData(A<CourseSearchProperties>.Ignored)).Returns(returnedCourseData);

            //Act
            var result = findACourseService.GetFilteredData(courseSearchFilters, CourseSearchOrderBy.Relevance, 1).Result;

            //Assert
            A.CallTo(() => repository.RetrieveData(A<CourseSearchProperties>.Ignored)).MustHaveHappenedOnceExactly();
            A.Equals(result, returnedCourseData);
        }

        [Test]
        public void CheckEnumOfCourseTypeIsReturned() 
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<CourseType>() { CourseType.All, CourseType.ClassroomBased, 
                                                    CourseType.ClassroomBased, CourseType.WorkBased };

            A.CallTo(() => repository.GetFilter<CourseType>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<CourseType>();

            //Assert
            A.Equals(result.Count, 4) ;
        }

        [Test]
        public void CheckEnumOfCourseHoursIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<CourseHours>() { CourseHours.All, CourseHours.Flexible,
                                                    CourseHours.Fulltime, CourseHours.PartTime };

            A.CallTo(() => repository.GetFilter<CourseHours>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<CourseHours>();

            //Assert
            A.Equals(result.Count, 4);
        }

        [Test]
        public void CheckEnumOfCourseStudyTypeIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<StudyMode>() { StudyMode.Flexible, StudyMode.FullTime,
                                                    StudyMode.PartTime, StudyMode.Undefined };

            A.CallTo(() => repository.GetFilter<StudyMode>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<StudyMode>();

            //Assert
            A.Equals(result.Count, 4);
        }

        [Test]
        public void CheckEnumOfCourseStartDateIsReturned()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            var findACourseService = new FindACourseService(repository);
            var returnList = new List<StartDate>() { StartDate.Anytime, StartDate.FromToday,
                                                    StartDate.SelectDateFrom };

            A.CallTo(() => repository.GetFilter<StartDate>()).Returns(returnList);

            //Act
            var result = findACourseService.GetFilterByName<StartDate>();

            //Assert
            A.Equals(result.Count, 3);
            A.Equals(result.GetType(), typeof(List<StartDate>));
        }

        [Test]
        public void EnsureCourseDetailsReturnsData()
        {
            //Arrange
            var repository = A.Fake<IFindACourseRepository>();
            string courseId = @"c0a5dfeb-f2a6-4000-8272-ec1fa78df265";
            string runId = @"6707d15a-5a19-4c18-9cc8-570573bb5d67";

            var returnedCourseDetails = new CourseDetails();
            returnedCourseDetails = new CourseDetails
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
            A.Equals(result, returnedCourseDetails);
        }
    }
}