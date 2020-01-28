using DFC.App.FindACourse.Data.Enums;
using System;
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
    }
}