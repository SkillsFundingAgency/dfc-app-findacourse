using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.ViewModels
{
    public class CourseRegion
    {
        public string Name { get; set; }

        public IList<string> SubRegions { get; set; }
    }
}
