using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.FindACourseClient;

namespace DFC.App.FindACourse.Services
{
    public interface IFindACourseService
    {
        Task<bool> PingAsync();

        IList<T> GetFilterByName<T>();

        Task<CourseSearchResult> GetFilteredData(CourseSearchFilters filters, CourseSearchOrderBy orderBy, int pageSize);

        Task<CourseSearchResult> GetSortedData(CourseSearchOrderBy orderBy, int pageSize);
    }
}