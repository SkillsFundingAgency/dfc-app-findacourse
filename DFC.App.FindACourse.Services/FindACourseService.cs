using DFC.App.FindACourse.Repository;
using DFC.FindACourseClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public class FindACourseService : IFindACourseService
    {
        private readonly IFindACourseRepository repository;

        public FindACourseService(IFindACourseRepository repository)
        {
            this.repository = repository;
        }

        public bool PingAsync()
        {
            return repository.PingAsync();
        }

        public IList<T> GetFilterByName<T>()
        {
            return repository.GetFilter<T>();
        }

        public async Task<CourseSearchResult> GetFilteredData(CourseSearchFilters filters, CourseSearchOrderBy orderBy, int pageSize)
        {
            var criteriaProperties = new CourseSearchProperties
            {
                Filters = filters,
                OrderedBy = orderBy,
                Page = pageSize,
            };

            return await repository.RetrieveData(criteriaProperties).ConfigureAwait(true);
        }

        public async Task<CourseDetails> GetCourseDetails(string courseId, string runId)
        {
            return await repository.GetCourseDetails(courseId, runId).ConfigureAwait(false);
        }
    }
}