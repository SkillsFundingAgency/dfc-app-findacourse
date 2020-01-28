using System.Collections.Generic;
using DFC.App.FindACourse.Data.Domain;

namespace DFC.App.FindACourse.ViewModels
{
    public class FiltersListViewModel
    {
        /// <summary>
        ///     Gets or Sets the filter name, not the title.
        /// </summary>
        public string FilterText { get; set; }

        /// <summary>
        ///     Gets or Sets the filter Title.
        /// </summary>
        public string FilterTitle { get; set; }

        /// <summary>
        ///     Gets or Sets the Filter content.
        /// </summary>
        public List<Filter> LstFilter { get; set; }
    }
}
