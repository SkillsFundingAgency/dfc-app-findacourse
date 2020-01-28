using DFC.App.FindACourse.Data.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Repository
{
    public class FindACourseRepository : IFindACourseRepository
    {
        public async Task<bool> PingAsync()
        {
            return true;
        }

        public IList<T> GetFilter<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public async Task<IList> RetrieveData()
        {


            // filter the results
            throw new NotImplementedException();
        }

        public Task<IList> RetrieveSortedData(string criteria)
        {
            if (criteria == null)
            {
                throw new NullReferenceException("Search criteria is empty");
            }

            // sort the results here
            throw new NotImplementedException();
        }
    }
}