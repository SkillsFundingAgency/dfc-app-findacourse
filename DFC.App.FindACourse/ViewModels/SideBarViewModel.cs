using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DFC.App.FindACourse.ViewModels
{
    public class SideBarViewModel
    {
        public string TownOrPostcode { get; set; }

        public string DistanceValue { get; set; }

        public List<SelectListItem> DistanceOptions { get; set; }

        public FiltersListViewModel CourseStudyTime { get; set; }

        public FiltersListViewModel CourseHours { get; set; }
        
        public FiltersListViewModel CourseType { get; set; }
        
        public FiltersListViewModel StartDate { get; set; }
    }
}
