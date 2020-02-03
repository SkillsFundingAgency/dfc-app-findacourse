using DFC.App.FindACourse.Data.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFC.FindACourseClient;

namespace DFC.App.FindACourse.Repository
{
    public class FindACourseRepository : IFindACourseRepository
    {
        private readonly ICourseSearchApiService courseSearchApiService;

        public FindACourseRepository(ICourseSearchApiService courseSearchApiService)
        {
            this.courseSearchApiService = courseSearchApiService;
        }

        public async Task<bool> PingAsync()
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
    }
}