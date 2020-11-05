using DFC.App.FindACourse.Data.Contracts;
using DFC.App.FindACourse.Data.Enums;
using DFC.App.FindACourse.Data.Models;
using DFC.Compui.Cosmos.Contracts;
using DFC.Compui.Cosmos.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public class WebhooksService : IWebhooksService
    {
        private readonly ILogger<WebhooksService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService<StaticContentItemModel> eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly IStaticContentReloadService staticContentReloadService;
        private readonly IContentCacheService contentCacheService;
        private readonly ISharedContentService sharedContentService;

        public WebhooksService(
            ILogger<WebhooksService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<StaticContentItemModel> eventMessageService,
            ICmsApiService cmsApiService,
            IStaticContentReloadService staticContentReloadService,
            IContentCacheService contentCacheService,
            ISharedContentService sharedContentService)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.staticContentReloadService = staticContentReloadService;
            this.contentCacheService = contentCacheService;
            this.sharedContentService = sharedContentService;
        }

        public async Task<HttpStatusCode> ProcessMessageAsync(WebhookCacheOperation webhookCacheOperation, Guid eventId, Guid sharedContentId, string apiEndpoint)
        {
            switch (webhookCacheOperation)
            {
                case WebhookCacheOperation.Delete:
                        return await DeleteContentItemAsync(sharedContentId).ConfigureAwait(false);

                case WebhookCacheOperation.CreateOrUpdate:
                        if (!Uri.TryCreate(apiEndpoint, UriKind.Absolute, out Uri? url))
                        {
                            throw new InvalidDataException($"Invalid Api url '{apiEndpoint}' received for Event Id: {eventId}");
                        }

                        return await ProcessContentAsync(sharedContentId, CancellationToken.None).ConfigureAwait(false);

                default:
                        logger.LogError($"Event Id: {eventId} got unknown cache operation - {webhookCacheOperation}");
                        return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> DeleteContentItemAsync(Guid contentItemId)
        {
            var sharedContent = await sharedContentService.GetById(contentItemId).ConfigureAwait(false);

            if (sharedContent != null)
            {
                // Return httpstatuscode
                return await sharedContentService.RemoveContentItem(contentItemId).ConfigureAwait(false);
            }
            return HttpStatusCode.NoContent;
        }

        public async Task<HttpStatusCode> ProcessContentAsync(Guid sharedContentId, CancellationToken stoppingToken)
        {
            var sharedContent = await sharedContentService.GetById(sharedContentId).ConfigureAwait(false);

            logger.LogInformation("Process summary list started");

            contentCacheService.Clear();

            if (stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("Process summary list cancelled");

                return HttpStatusCode.NotAcceptable;
            }

            await GetAndSaveItemAsync(sharedContent, stoppingToken).ConfigureAwait(false);

            return HttpStatusCode.OK;
        }

        public async Task GetAndSaveItemAsync(StaticContentItemModel item, CancellationToken stoppingToken)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            item.PartitionKey = "/";
            item.CanonicalName = item.skos_prefLabel.Replace(" ", "").ToLower();
            try
            {
                logger.LogInformation($"Updating static content cache with {item.Id} - {item.Url}");

                var result = await eventMessageService.UpdateAsync(item).ConfigureAwait(false);

                if (result == HttpStatusCode.NotFound)
                {
                    logger.LogInformation($"Does not exist, creating static content cache with {item.Id} - {item.Url}");

                    result = await eventMessageService.CreateAsync(item).ConfigureAwait(false);

                    if (result == HttpStatusCode.OK)
                    {
                        logger.LogInformation($"Created static content cache with {item.Id} - {item.Url}");
                    }
                    else
                    {
                        logger.LogError($"Static content cache create error status {result} from {item.Id} - {item.Url}");
                    }
                }
                else
                {
                    logger.LogInformation($"Updated static content cache with {item.Id} - {item.Url}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in get and save for {item.Id} - {item.Url}");
            }
        }
    }
}
