using DFC.App.FindACourse.Data.Models;
using DFC.CompositeInterfaceModels.FindACourseClient;

namespace DFC.App.FindACourse.ViewModels
{
    public class DetailsViewModel
    {
        public CourseDetails CourseDetails { get; set; }

        public string SearchTerm { get; set; }

        public StaticContentItemModel SpeakToAnAdvisor { get; set; }
    }
}
