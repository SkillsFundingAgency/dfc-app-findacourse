using DFC.App.FindACourse.Data.Models;
using DFC.CompositeInterfaceModels.FindACourseClient;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class DetailsViewModel
    {
        public DetailsViewModel()
        {
            DetailsRightBarViewModel = new DetailsRightBarViewModel();
        }

        public CourseDetails CourseDetails { get; set; }

        public string SearchTerm { get; set; }

        public IList<CourseRegion> CourseRegions { get; set; }

        public DetailsRightBarViewModel DetailsRightBarViewModel { get; set; }
        public string BackLinkUrl { get; set; }
    }
}
