using DFC.App.FindACourse.Data.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.Data.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class ListFilters
    {
        public static List<SelectListItem> GetDistanceList()
        {
            var lstDistance = new List<SelectListItem>
            {
                new SelectListItem("5 miles", "5 miles"),
                new SelectListItem("10 miles", "10 miles", true),
                new SelectListItem("15 miles", "15 miles"),
                new SelectListItem("30 miles", "30 miles"),
                new SelectListItem("45 miles", "45 miles"),
                new SelectListItem("60 miles", "60 miles"),
                new SelectListItem("75 miles", "75 miles"),
            };

            return lstDistance;
        }

        public static List<Filter> GetLearningMethodList()
        {
            var lstLearningMethod = new List<Filter>
            {
                new Filter { Id = "Online", Text = "Online" },
                new Filter { Id = "Classroom based", Text = "Classroom based" },
                new Filter { Id = "Work based", Text = "Work based" },
                new Filter { Id = "Blended Learning", Text = "Blended learning" },
            };

            return lstLearningMethod;
        }

        public static List<Filter> GetCourseTypeList()
        {
            var courseTypes = new List<Filter>
            {
                new Filter { Id = "Essential Skills", Text = "Essential skills" },
                new Filter { Id = "T Levels", Text = "T-levels" },
                new Filter { Id = "HTQs", Text = "HTQs" },
                new Filter { Id = "Free courses for Jobs", Text = "Free courses for Jobs" },
                new Filter { Id = "Multiply", Text = "Multiply" },
                new Filter { Id = "Skills Bootcamp", Text = "Skills bootcamp" },
            };

            return courseTypes;
        }

        public static List<Filter> GetHoursList()
        {
            var lstCourseHours = new List<Filter>
            {
                new Filter { Id = "Full time", Text = "Full time" },
                new Filter { Id = "Part time", Text = "Part time" },
                new Filter { Id = "Flexible", Text = "Flexible" },
            };

            return lstCourseHours;
        }

        public static List<Filter> GetStudyTimeList()
        {
            var lstCourseStudyTime = new List<Filter>
            {
                new Filter { Id = "Daytime", Text = "Daytime" },
                new Filter { Id = "Evening", Text = "Evening" },
                new Filter { Id = "Weekend", Text = "Weekend" },
                new Filter { Id = "Day or block release", Text = "Day or block release" },
            };

            return lstCourseStudyTime;
        }

        public static List<Filter> GetLevelList()
        {
            return new List<Filter>()
            {
                new Filter { Id = "E", Text = "Entry level" },
                new Filter { Id = "1", Text = "Level 1" },
                new Filter { Id = "2", Text = "Level 2" },
                new Filter { Id = "3", Text = "Level 3" },
                new Filter { Id = "4", Text = "Level 4" },
                new Filter { Id = "5", Text = "Level 5" },
                new Filter { Id = "6", Text = "Level 6" },
                new Filter { Id = "7", Text = "Level 7" },
            };
        }

        public static List<SelectListItem> GetStartDateList()
        {
            var lstStartDate = new List<SelectListItem>
            {
                new SelectListItem("Anytime", "Anytime", true),
                new SelectListItem("Next 3 months", "Next 3 months"),
                new SelectListItem("In 3 to 6 months", "In 3 to 6 months"),
                new SelectListItem("More than 6 months", "More than 6 months"),
            };

            return lstStartDate;
        }

        public static List<SelectListItem> GetOrderByOptions()
        {
            var lstOrderBy = new List<SelectListItem>
            {
                new SelectListItem("Distance", "Distance"),
                new SelectListItem("Relevance", "Relevance", true),
                new SelectListItem("Start date", "Start date"),
            };

            return lstOrderBy;
        }
    }
}
