using DFC.FindACourseClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Repository
{
    public interface IFindACourseRepository
    {
        /// <summary>
        ///     Pings the service to ensure it is running.
        /// </summary>
        /// <returns>type bool.</returns>
        bool PingAsync();

        /// <summary>
        ///     Creates a list from any enum type.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <returns>IList of enum types.</returns>
        IList<T> GetFilter<T>();

        /// <summary>
        ///     Retrieves data from FAC API for a given criteria.
        /// </summary>
        /// <param name="properties">CourseSearchProperties object.</param>
        /// <returns>Task of CourseSearchResult.</returns>
        Task<CourseSearchResult> RetrieveData(CourseSearchProperties properties);
    }
}