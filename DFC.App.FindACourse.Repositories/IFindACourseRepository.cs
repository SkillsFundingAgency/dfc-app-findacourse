using DFC.CompositeInterfaceModels.FindACourseClient;
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

        /// <summary>
        ///     Gets the course details.
        /// </summary>
        /// <param name="id">Course Id.</param>
        /// <param name="runId">Course Run Id.</param>
        /// <returns>Course Details.</returns>
        Task<CourseDetails> GetCourseDetails(string id, string runId);

        Task<TLevelDetails> GetTLevelDetails(string tlevelId);
    }
}