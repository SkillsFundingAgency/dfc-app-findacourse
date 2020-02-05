using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.FindACourseClient;

namespace DFC.App.FindACourse.Services
{
    public interface IFindACourseService
    {
        /// <summary>
        ///     Pings the service to ensure it is running.
        /// </summary>
        /// <returns>Task of type bool.</returns>
        Task<bool> PingAsync();

        /// <summary>
        ///     Creates a list from any enum type.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <returns>IList of enum types.</returns>
        IList<T> GetFilterByName<T>();

        /// <summary>
        ///     Retrieves data from FAC API for a given criteria.
        /// </summary>
        /// <param name="filters">Search criteria.</param>
        /// <param name="orderBy">Order by type.</param>
        /// <param name="pageSize">page requested.<./param>
        /// <returns>Task of CourseSearchResult.</returns>
        Task<CourseSearchResult> GetFilteredData(CourseSearchFilters filters, CourseSearchOrderBy orderBy, int pageSize);
    }
}