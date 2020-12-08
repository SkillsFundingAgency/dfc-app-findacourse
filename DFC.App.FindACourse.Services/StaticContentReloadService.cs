using DFC.App.FindACourse.Data.Contracts;
using DFC.App.FindACourse.Data.Models;
using DFC.Content.Pkg.Netcore.Data.Contracts;
using DFC.Content.Pkg.Netcore.Data.Models.ClientOptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public class StaticContentReloadService : IStaticContentReloadService
    {
        private readonly ILogger<StaticContentReloadService> logger;
        private readonly AutoMapper.IMapper mapper;
        private readonly IEventMessageService<StaticContentItemModel> eventMessageService;
        private readonly ICmsApiService cmsApiService;
        private readonly CmsApiClientOptions cmsApiClientOptions;

        public StaticContentReloadService(
            ILogger<StaticContentReloadService> logger,
            AutoMapper.IMapper mapper,
            IEventMessageService<StaticContentItemModel> eventMessageService,
            ICmsApiService cmsApiService,
            CmsApiClientOptions cmsApiClientOptions)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.eventMessageService = eventMessageService;
            this.cmsApiService = cmsApiService;
            this.cmsApiClientOptions = cmsApiClientOptions;
        }

        public async Task Reload(CancellationToken stoppingToken)
        {
            try
            {
                logger.LogInformation("Reload static content started");

                var contentIds = cmsApiClientOptions.ContentIds.Split(",", StringSplitOptions.RemoveEmptyEntries);
                var apiDataModels = new List<StaticContentItemModel>();

                foreach (var contentId in contentIds)
                {
                    var apiDataModel = await cmsApiService.GetItemAsync<StaticContentItemModel>("sharedcontent", new Guid(contentId)).ConfigureAwait(false);

                    if (apiDataModel != null)
                    {
                        apiDataModels.Add(apiDataModel);
                    }
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogWarning("Reload static content cancelled");

                    return;
                }

                if (apiDataModels.Any())
                {
                    await ProcessContentAsync(apiDataModels, stoppingToken).ConfigureAwait(false);

                    if (stoppingToken.IsCancellationRequested)
                    {
                        logger.LogWarning("Reload static content cancelled");

                        return;
                    }
                }

                logger.LogInformation("Reload static content completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in static content reload");
            }
        }

        public async Task ProcessContentAsync(List<StaticContentItemModel> sharedContent, CancellationToken stoppingToken)
        {
            logger.LogInformation("Process summary list started");

            if (stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("Process summary list cancelled");

                return;
            }

            await GetAndSaveItemAsync(sharedContent, stoppingToken).ConfigureAwait(false);

            logger.LogInformation("Process summary list completed");
        }

        public async Task GetAndSaveItemAsync(List<StaticContentItemModel> items, CancellationToken stoppingToken)
        {
            _ = items ?? throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
            {
                try
                {
                    logger.LogInformation($"Updating static content cache with {item.Id} - {item.Url}");

                    item.PartitionKey = item.PageLocation;
                    item.CanonicalName = item.skos_prefLabel.Replace(" ", "-").ToLower();
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
                    logger.LogError(ex, $"Error in get and save for {item?.Id} - {item?.Url}");
                }
            }
        }
    }
}
