using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class AjaxModel
    {
        public string HTML { get; set; }

        public int Count { get; set; }

        public bool? ShowDistanceSelector { get; set; }
    }
}
