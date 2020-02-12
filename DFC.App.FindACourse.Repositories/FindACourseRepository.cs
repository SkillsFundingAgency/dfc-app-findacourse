using DFC.FindACourseClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Repository
{
    public class FindACourseRepository : IFindACourseRepository
    {
        private readonly ICourseSearchApiService courseSearchApiService;

        public FindACourseRepository(ICourseSearchApiService courseSearchApiService)
        {
            this.courseSearchApiService = courseSearchApiService;
        }

        public bool PingAsync()
        {
            return true;
        }

        public IList<T> GetFilter<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public async Task<CourseSearchResult> RetrieveData(CourseSearchProperties properties)
        {
            return await courseSearchApiService.SearchCoursesAsync(properties).ConfigureAwait(false);
        }

        public async Task<CourseDetails> GetCourseDetails(string id, string runId)
        {
            return await courseSearchApiService.GetCourseDetailsAsync(id, runId).ConfigureAwait(false);
        }
    }
}