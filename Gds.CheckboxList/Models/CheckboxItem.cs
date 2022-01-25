using System.Diagnostics.CodeAnalysis;

namespace GdsCheckboxList.Models
{
    /// <summary>
    /// Model class used to build the check box list
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CheckBoxItem
    {
        public CheckBoxItem(object id, string title, bool isChecked = false, bool isDisabled = false)
        {
            Id = id;
            Title = title;
            IsChecked = isChecked;
            IsDisabled = isDisabled;
        }

        /// <summary>
        /// Used to indicate the value attribute of the checkbox input
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        /// Used to indicate the title text of the checkbox input
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Used to set checked attribute of the checkbox input
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// Used to set disabled attribute of the checkbox input
        /// </summary>
        public bool IsDisabled { get; set; }
    }
}
