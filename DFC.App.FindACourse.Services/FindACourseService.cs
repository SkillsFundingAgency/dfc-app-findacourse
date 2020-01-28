using System.Collections.Generic;
using DFC.App.FindACourse.Repository;
using System.Threading.Tasks;
using System.Collections;

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

        public async Task<IList> GetFilteredData()
        {
            return await repository.RetrieveData();
        }

        public async Task<IList> GetSortedData(string criteria)
        {
            return await repository.RetrieveSortedData(criteria);
        }
    }
}