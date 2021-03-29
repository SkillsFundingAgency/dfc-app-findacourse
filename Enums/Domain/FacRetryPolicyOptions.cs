using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.Data.Domain
{
    [ExcludeFromCodeCoverage]
    public class FacRetryPolicyOptions
    {
        public int Count { get; set; } = 3;

        public int BackoffPower { get; set; } = 2;
    }
}
