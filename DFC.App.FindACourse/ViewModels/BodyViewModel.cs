using DFC.FindACourseClient;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DFC.App.FindACourse.ViewModels
{
    public class BodyViewModel
    {
        public string CurrentSearchTerm { get; set; }

        public HtmlString Content { get; set; } = new HtmlString("Unknown Find a course content");

        public SideBarViewModel SideBar { get; set; }
       
        public CourseSearchResult Results { get; set; }

        public string SelectedOrderByValue { get; set; }

        public List<SelectListItem> OrderByOptions { get; set; }
    }
}