namespace DFC.App.FindACourse.Data.Domain
{
    public class FacRetryPolicyOptions
    {
        public int Count { get; set; } = 3;

        public int BackoffPower { get; set; } = 2;
    }
}
