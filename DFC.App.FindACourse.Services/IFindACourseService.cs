using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public interface IFindACourseService
    {
        Task<bool> PingAsync();
    }
}