using System;
using System.Collections.Generic;
using System.Text;

namespace DFC.App.FindACourse.Data.Domain
{
    public class FacPolicyOptions
    {
        public FacCircuitBreakerPolicyOptions HttpCircuitBreaker { get; set; }

        public FacRetryPolicyOptions HttpRetry { get; set; }
    }
}
