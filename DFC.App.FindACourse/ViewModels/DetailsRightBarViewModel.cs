using DFC.App.FindACourse.Data.Models;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class DetailsRightBarViewModel
    {
        public string SpeakToAnAdviser { get; set; }

        public ProviderViewModel Provider { get; set; }
    }
}
