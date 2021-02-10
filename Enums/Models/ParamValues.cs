using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class ParamValues
    {
        public string SearchTerm { get; set; }

        public string Town { get; set; }

        public string Distance { get; set; }

        public string CourseType { get; set; }

        public string CourseHours { get; set; }

        public string CourseStudyTime { get; set; }

        public string StartDate { get; set; }

        public int Page { get; set; }

        public bool FilterA { get; set; }

        public bool IsTest { get; set; }

        public string OrderByValue { get; set; }

        public int D { get; set; }
    }
}
