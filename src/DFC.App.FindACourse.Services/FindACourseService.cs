using DFC.App.FindACourse.Repository;
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

        public async Task<bool> PingAsync()
        {
            return await repository.PingAsync().ConfigureAwait(false);
        }
    }
}