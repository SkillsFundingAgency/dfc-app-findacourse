using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Services
{
    public interface IFindACourseService
    {
        Task<bool> PingAsync();

        IList<T> GetFilterByName<T>();

        Task<IList> GetFilteredData();

        Task<IList> GetSortedData(string criteria);
    }
}