using System.Collections.Generic;
using GdsCheckboxList.Models;
using DFC.App.FindACourse.Data.Domain;

namespace DFC.App.FindACourse.ViewModels
{
    public class FiltersListViewModel
    {
        public FiltersListViewModel()
        {
            this.selectedIds = new List<string>();
        }

        public string FilterText { get; set; }

        public string FilterTitle { get; set; }

        public List<CheckBoxItem> lstChkFilter { get; set; }

        public List<string> selectedIds { get; set; }
    }
}
