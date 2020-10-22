using DFC.App.FindACourse.Data.Enums;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Data.Contracts
{
    public interface IWebhooksService
    {
        Task<HttpStatusCode> DeleteContentItemAsync(Guid contentItemId);

        Task<HttpStatusCode> ProcessContentAsync(Guid sharedContentId, CancellationToken stoppingToken);

        Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid sharedContentId, string apiEndpoint);
    }
}
