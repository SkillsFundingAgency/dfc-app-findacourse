using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AjaxModel
    {
        public string HTML { get; set; }

        public int Count { get; set; }

        public bool? ShowDistanceSelector { get; set; }

        public List<LocationSuggestViewModel> DidYouMeanLocations { get; set; }

        public string AutoSuggestedTown { get; set; }

        public string AutoSuggestedCoordinates { get; set; }

        public bool UsingAutoSuggestedLocation { get; set; }
    }
}
