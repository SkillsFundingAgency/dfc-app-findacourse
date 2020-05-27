using DFC.CompositeInterfaceModels.FindACourseClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public interface IFindACourseService
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
        IList<T> GetFilterByName<T>();

        /// <summary>
        ///     Retrieves data from FAC API for a given criteria.
        /// </summary>
        /// <param name="filters">Search criteria.</param>
        /// <param name="orderBy">Order by type.</param>
        /// <param name="pageSize">page requested.<./param>
        /// <returns>Task of CourseSearchResult.</returns>
        Task<CourseSearchResult> GetFilteredData(CourseSearchFilters filters, CourseSearchOrderBy orderBy, int pageSize);

        /// <summary>
        ///     Retrieves the course details for a given course and period.
        /// </summary>
        /// <param name="courseId">Course Id.</param>
        /// <param name="runId">Run Id.</param>
        /// <returns>Task of course details.</returns>
        Task<CourseDetails> GetCourseDetails(string courseId, string runId);
    }
}