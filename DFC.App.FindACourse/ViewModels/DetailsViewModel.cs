using DFC.App.FindACourse.Data.Models;
using DFC.CompositeInterfaceModels.FindACourseClient;
using System.Collections.Generic;

namespace DFC.App.FindACourse.ViewModels
{
    public class DetailsViewModel
    {
        public CourseDetails CourseDetails { get; set; }

        public string SearchTerm { get; set; }

        public StaticContentItemModel SpeakToAnAdviser { get; set; }

        public IList<CourseRegion> CourseRegions { get; set; }
    }
}
