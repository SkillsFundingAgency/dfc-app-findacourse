using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DFC.App.FindACourse.Data.Domain
{
    [ExcludeFromCodeCoverage]
    public class FacPolicyOptions
    {
        public FacCircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        public FacRetryPolicyOptions HttpRetry { get; set; }
    }
}
