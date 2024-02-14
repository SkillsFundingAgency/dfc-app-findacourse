using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class SideBarViewModel
    {
        public string TownOrPostcode { get; set; }

        public string DistanceValue { get; set; }

        public string StartDateValue { get; set; }

        public string CurrentSearchTerm { get; set; }

        public bool FiltersApplied { get; set; }

        public string SelectedOrderByValue { get; set; }

        public List<SelectListItem> OrderByOptions { get; set; }

        public List<SelectListItem> DistanceOptions { get; set; }

        public FiltersListViewModel CourseStudyTime { get; set; }

        public FiltersListViewModel QualificationLevels { get; set; }

        public FiltersListViewModel CourseHours { get; set; }

        public FiltersListViewModel LearningMethod { get; set; }

        public List<SelectListItem> StartDateOptions { get; set; }

        public int D { get; set; }

        public string Coordinates { get; set; }

        public List<LocationSuggestViewModel> DidYouMeanLocations { get; set; }

        public string SuggestedLocation { get; set; }
    }
}
