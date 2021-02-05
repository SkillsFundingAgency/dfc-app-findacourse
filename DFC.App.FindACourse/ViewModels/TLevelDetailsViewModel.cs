using DFC.App.FindACourse.Data.Models;
using DFC.CompositeInterfaceModels.FindACourseClient;
using System.Collections.Generic;

namespace DFC.App.FindACourse.ViewModels
{
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
