using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.FindACourse.Data.Models
{
    [ExcludeFromCodeCoverage]
    public class EventGridEventData
    {
        public string? Api { get; set; }

        public string? ItemId { get; set; }

        public string? VersionId { get; set; }

        public string? DisplayText { get; set; }

        public string? Author { get; set; }
    }
}
