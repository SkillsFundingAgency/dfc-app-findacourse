using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.Data.Domain
{
    [ExcludeFromCodeCoverage]
    public class Filter
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public bool Selected { get; set; }
    }
}
