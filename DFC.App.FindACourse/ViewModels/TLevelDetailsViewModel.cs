using DFC.CompositeInterfaceModels.FindACourseClient;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class TLevelDetailsViewModel
    {
        public TLevelDetailsViewModel()
        {
            DetailsRightBarViewModel = new DetailsRightBarViewModel();
        }

        public TLevelDetails TlevelDetails { get; set; }

        public string SearchTerm { get; set; }

        public DetailsRightBarViewModel DetailsRightBarViewModel { get; set; }
    }
}
