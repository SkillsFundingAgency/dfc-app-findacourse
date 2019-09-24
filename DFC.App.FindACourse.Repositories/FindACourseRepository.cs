using System.Threading.Tasks;

namespace DFC.App.FindACourse.Repository
{
    public class FindACourseRepository : IFindACourseRepository
    {
        public async Task<bool> PingAsync()
        {
            return true;
        }
    }
}