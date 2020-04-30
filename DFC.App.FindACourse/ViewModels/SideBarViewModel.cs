using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DFC.App.FindACourse.ViewModels
{
    public class SideBarViewModel
    {
        public string TownOrPostcode { get; set; }

        public string DistanceValue { get; set; }

        public string StartDateValue { get; set; }

        public string CurrentSearchTerm { get; set; }

        public bool FiltersApplied { get; set; }

        public List<SelectListItem> DistanceOptions { get; set; }

        public FiltersListViewModel CourseStudyTime { get; set; }

        public FiltersListViewModel CourseHours { get; set; }

        public FiltersListViewModel CourseType { get; set; }

        public List<SelectListItem> StartDateOptions { get; set; }

    }
}
