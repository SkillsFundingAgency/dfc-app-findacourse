using System;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.Data.Domain
{
    [ExcludeFromCodeCoverage]
    public class CourseSearchSettings
    {
        public Uri CourseSearchUrl { get; set; }

        public string HealthCheckKeyWords { get; set; }
    }
}
