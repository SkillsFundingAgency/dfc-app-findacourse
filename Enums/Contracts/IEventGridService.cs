using DFC.App.FindACourse.Data.Enums;
using DFC.App.FindACourse.Data.Models;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Data.Contracts
{
    public interface IEventGridService
    {
        Task CompareAndSendEventAsync(StaticContentItemModel? existingContentPageModel, StaticContentItemModel? updatedContentPageModel);

        Task SendEventAsync(WebhookCacheOperation webhookCacheOperation, StaticContentItemModel? updatedContentPageModel);
    }
}
