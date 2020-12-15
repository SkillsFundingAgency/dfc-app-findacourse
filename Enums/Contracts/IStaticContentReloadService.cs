using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Data.Contracts
{
    public interface IStaticContentReloadService
    {
        Task Reload(CancellationToken stoppingToken);
    }
}
