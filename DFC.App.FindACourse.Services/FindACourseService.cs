using DFC.App.FindACourse.Repository;
using DFC.CompositeInterfaceModels.FindACourseClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fac = DFC.FindACourseClient;

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

            return await repository.RetrieveData(criteriaProperties).ConfigureAwait(false);
        }

        public async Task<CourseDetails> GetCourseDetails(string courseId, string runId)
        {
            return await repository.GetCourseDetails(courseId, runId).ConfigureAwait(false);
        }

        public async Task<TLevelDetails> GetTLevelDetails(string tlevelId, string tlevelLocationId)
        {
            var tlevelDetails = await repository.GetTLevelDetails(tlevelId).ConfigureAwait(false);

            //put the requested venue at the top if possible
            tlevelDetails.Venues = tlevelDetails.Venues?.OrderByDescending(m => m.Id == tlevelLocationId).ToList();
            return tlevelDetails;
        }

        public async Task<List<Fac.Sector>> GetSectors()
        {
            return await repository.GetSectors().ConfigureAwait(false);
        }
    }
}