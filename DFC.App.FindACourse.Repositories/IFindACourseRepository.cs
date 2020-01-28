using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DFC.App.FindACourse.Data.Enums;

namespace DFC.App.FindACourse.Repository
{
    public interface IFindACourseRepository
    {
        Task<bool> PingAsync();

        IList<T> GetFilter<T>();

        Task<IList> RetrieveData();
        
        Task<IList> RetrieveSortedData(string criteria);
    }
}