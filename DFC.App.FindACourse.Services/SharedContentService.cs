using AutoMapper;
using DFC.App.FindACourse.Data.Contracts;
using DFC.App.FindACourse.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public class SharedContentService : ISharedContentService
    {
        private readonly IStaticCosmosRepository<StaticContentItemModel> repository;
        private readonly IMapper mapper;

        public SharedContentService(
            IStaticCosmosRepository<StaticContentItemModel> repository,
            IMapper mapper)
        {
            this.repository = repository;
        }

        public async Task<StaticContentItemModel> GetByNameAsync(string canonicalName)
        {
            if (string.IsNullOrWhiteSpace(canonicalName))
            {
                throw new ArgumentNullException(nameof(canonicalName));
            }

            return await repository.GetAsync(d => d.CanonicalName == canonicalName.ToLowerInvariant()).ConfigureAwait(false);
        }

        public async Task<StaticContentItemModel> GetById(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await repository.GetAsync(d => d.Id == id).ConfigureAwait(false);
        }

        public async Task<HttpStatusCode> RemoveContentItem(Guid id)
        {
            return await repository.DeleteAsync(id).ConfigureAwait(false);
        }

        public async Task<List<StaticContentItemModel>> GetByNamesAsync(List<string> contentList)
        {
            var contentListItems = new List<StaticContentItemModel>();

            if (contentList == null)
            {
                throw new ArgumentNullException(nameof(contentList));
            }

            foreach (var item in contentList)
            {
                var sharedContentItem = await repository.GetAsync(d => d.CanonicalName == item.ToLowerInvariant()).ConfigureAwait(false);
                if (sharedContentItem != null)
                {
                    contentListItems.Add(sharedContentItem);
                }
            }

            return contentListItems;
        }
    }
}
