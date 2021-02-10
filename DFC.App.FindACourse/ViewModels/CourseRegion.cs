using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class CourseRegion
    {
        public string Name { get; set; }

        public IList<string> SubRegions { get; set; }
    }
}
