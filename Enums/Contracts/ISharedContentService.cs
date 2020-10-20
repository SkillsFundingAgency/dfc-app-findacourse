using DFC.App.FindACourse.Data.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Data.Contracts
{
    public interface ISharedContentService
    {
        Task<StaticContentItemModel> GetByNameAsync(string canonicalName);

        Task<StaticContentItemModel> GetById(Guid id);

        Task<List<StaticContentItemModel>> GetByNamesAsync(List<string> contentList);

        Task<HttpStatusCode> RemoveContentItem(Guid id);
    }
}
