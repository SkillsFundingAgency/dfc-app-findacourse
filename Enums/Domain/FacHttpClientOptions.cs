using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.FindACourse.Data.Domain
{
    [ExcludeFromCodeCoverage]
    public class FacHttpClientOptions
    {
        public Uri BaseAddress { get; set; }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 10);         // default to 10 seconds

        public string ApiKey { get; set; }
    }
}
