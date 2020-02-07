using GdsCheckboxList.Models;
using System.Collections.Generic;

namespace DFC.App.FindACourse.ViewModels
{
    public class FiltersListViewModel
    {
        public FiltersListViewModel()
        {
            this.SelectedIds = new List<string>();
        }

        public string FilterText { get; set; }

        public string FilterTitle { get; set; }

        public List<CheckBoxItem> LstChkFilter { get; set; }

        public List<string> SelectedIds { get; set; }
    }
}
