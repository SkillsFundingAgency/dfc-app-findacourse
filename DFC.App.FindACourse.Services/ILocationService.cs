using DFC.App.FindACourse.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public interface ILocationService
    {
        Task<IEnumerable<SearchLocationIndex>> GetSuggestedLocationsAsync(string term);
    }
}
