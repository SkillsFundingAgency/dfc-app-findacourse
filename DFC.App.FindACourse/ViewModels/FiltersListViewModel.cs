using GdsCheckboxList.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.FindACourse.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class FiltersListViewModel
    {
        public FiltersListViewModel()
        {
            SelectedIds = new List<string>();
        }

        public string FilterText { get; set; }

        public string FilterTitle { get; set; }

        public List<CheckBoxItem> LstChkFilter { get; set; }

        public List<string> SelectedIds { get; set; }
    }
}
