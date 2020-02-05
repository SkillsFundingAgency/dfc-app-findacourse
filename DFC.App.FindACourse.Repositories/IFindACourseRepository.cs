using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.App.FindACourse.Data.Enums;
using DFC.FindACourseClient;

namespace DFC.App.FindACourse.Repository
{
    public interface IFindACourseRepository
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
        IList<T> GetFilter<T>();

        /// <summary>
        ///     Retrieves data from FAC API for a given criteria.
        /// </summary>
        /// <param name="properties">CourseSearchProperties object</param>
        /// <returns>Task of CourseSearchResult.</returns>
        Task<CourseSearchResult> RetrieveData(CourseSearchProperties properties);
    }
}