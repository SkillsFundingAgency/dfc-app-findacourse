using DFC.App.FindACourse.Data.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.FindACourse.Data.Helpers
{
    public static class ListFilters
    { 
        public static List<SelectListItem> GetDistanceList()
        {
            var lstDistance = new List<SelectListItem>();
            lstDistance.Add(new SelectListItem("5 miles", "5 miles"));
            lstDistance.Add(new SelectListItem("10 miles", "10 miles", true));
            lstDistance.Add(new SelectListItem("15 miles", "15 miles"));
            lstDistance.Add(new SelectListItem("30 miles", "30 miles"));
            lstDistance.Add(new SelectListItem("45 miles", "45 miles"));
            lstDistance.Add(new SelectListItem("60 miles", "60 miles"));
            lstDistance.Add(new SelectListItem("75 miles", "75 miles"));

            return lstDistance;
        }

        public static List<Filter> GetCourseTypeList()
        {
            var lstCourseType = new List<Filter>();
            lstCourseType.Add(new Filter { Id = "Online", Text = "Online" });
            lstCourseType.Add(new Filter { Id = "Classroom based", Text = "Classroom based" });
            lstCourseType.Add(new Filter { Id = "Work based", Text = "Work based" });

            return lstCourseType;
        }

        public static List<Filter> GetHoursList()
        {
            var lstCourseHours = new List<Filter>();
            lstCourseHours.Add(new Filter { Id = "Full time", Text = "Full time" });
            lstCourseHours.Add(new Filter { Id = "Part time", Text = "Part time" });
            lstCourseHours.Add(new Filter { Id = "Flexible", Text = "Flexible" });

            return lstCourseHours;
        }

        public static List<Filter> GetStudyTimeList()
        {
            var lstCourseStudyTime = new List<Filter>();
            lstCourseStudyTime.Add(new Filter { Id = "Daytime", Text = "Daytime" });
            lstCourseStudyTime.Add(new Filter { Id = "Evening", Text = "Evening" });
            lstCourseStudyTime.Add(new Filter { Id = "Weekend", Text = "Weekend" });
            lstCourseStudyTime.Add(new Filter { Id = "Day or block release", Text = "Day or block release" });

            return lstCourseStudyTime;
        }

        public static List<SelectListItem> GetStartDateList()
        {
            var lstStartDate = new List<SelectListItem>();

            lstStartDate.Add(new SelectListItem("Anytime", "Anytime", true));
            lstStartDate.Add(new SelectListItem("Next 3 months", "Next 3 months"));
            lstStartDate.Add(new SelectListItem("In 3 to 6 months", "In 3 to 6 months"));
            lstStartDate.Add(new SelectListItem("More than 6 months", "More than 6 months"));

            return lstStartDate;
        }

        public static List<SelectListItem> GetOrderByOptions()
        {
            var lstOrderBy = new List<SelectListItem>();

            lstOrderBy.Add(new SelectListItem("Distance", "Distance"));
            lstOrderBy.Add(new SelectListItem("Relevance", "Relevance", true));
            lstOrderBy.Add(new SelectListItem("Start date", "Start date"));

            return lstOrderBy;
        }
    }
}
