using DFC.CompositeInterfaceModels.FindACourseClient;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BodyViewModel
    {
        public string CurrentSearchTerm { get; set; }

        public HtmlString Content { get; set; } = new HtmlString("Unknown Find a course content");

        public SideBarViewModel SideBar { get; set; }

        public CourseSearchResult Results { get; set; }

        public string SelectedOrderByValue { get; set; }

        public string SelectedDistanceValue { get; set; }

        public List<SelectListItem> OrderByOptions { get; set; }

        public int RequestPage { get; set; }

        public bool IsNewPage { get; set; }

        public bool FromPaging { get; set; }

        public bool IsTest { get; set; }

        public CourseSearchFilters CourseSearchFilters { get; set; }

        public CourseSearchOrderBy CourseSearchOrderBy { get; set; }

        public bool IsResultBody { get; set; }

        public bool UsingAutoSuggestedLocation { get; set; }

        public bool FreeCourseSearch { get; set; }

        public string View { get; set; }
    }
}