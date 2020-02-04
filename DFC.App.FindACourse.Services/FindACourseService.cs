using System.Collections.Generic;
using DFC.App.FindACourse.Repository;
using System.Threading.Tasks;
using System.Collections;
using DFC.FindACourseClient;

namespace DFC.App.FindACourse.Services
{
    public class FindACourseService : IFindACourseService
    {
        private readonly IFindACourseRepository repository;

        public FindACourseService(IFindACourseRepository repository)
        {
            this.repository = repository;
        }

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
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
                Page = pageSize
            };

            var test = await repository.RetrieveData(criteriaProperties).ConfigureAwait(true);
            return test;
        }
    }
}