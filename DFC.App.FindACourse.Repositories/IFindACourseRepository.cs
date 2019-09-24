using System.Threading.Tasks;

namespace DFC.App.FindACourse.Repository
{
    public interface IFindACourseRepository
    {
        Task<bool> PingAsync();
    }
}