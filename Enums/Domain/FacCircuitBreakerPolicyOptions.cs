using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.FindACourse.Data.Domain
{
    [ExcludeFromCodeCoverage]
    public class FacCircuitBreakerPolicyOptions
    {
        public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);

        public int ExceptionsAllowedBeforeBreaking { get; set; } = 12;
    }
}
