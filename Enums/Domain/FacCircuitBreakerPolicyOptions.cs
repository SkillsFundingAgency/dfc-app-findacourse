using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.FindACourse.Data.Domain
{
    public class FacCircuitBreakerPolicyOptions
    {
        public TimeSpan DurationOfBreak { get; set; } = TimeSpan.FromSeconds(30);

        public int ExceptionsAllowedBeforeBreaking { get; set; } = 12;
    }
}
