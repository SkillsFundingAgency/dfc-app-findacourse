using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class BreadcrumbViewModel
    {
        public IList<BreadcrumbPathViewModel> Paths { get; set; }
    }
}