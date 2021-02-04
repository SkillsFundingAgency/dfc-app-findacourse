using GdsCheckboxList.Models;
using System.Collections.Generic;
using System.Text;

namespace GdsCheckboxList.Templates
{
    /// <summary>
    /// Basic template generator class used to generate basic HTML markup
    /// <inpu>
    /// </summary>
    internal class GDSTemplateGenerator : ITemplateGenerator
    {
        public string Generate(string name, List<CheckBoxItem> items)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append($"<div class=\"govuk-checkboxes govuk-checkboxes--small\">");

            for (int i = 0; i < items.Count; i++)
            {
                string checkedValue = items[i].IsChecked ? "checked" : string.Empty;
                string disabledValue = items[i].IsDisabled ? "disabled" : string.Empty;
                string nameValue = name;
                string idValue = name + $"[{i}]";
                string disabledClass = items[i].IsDisabled ? " disabled" : string.Empty;

                stringBuilder.Append(
                    $"<div class=\"govuk-checkboxes__item\">" +
                    $"<input class=\"govuk-checkboxes__input\" id=\"{idValue}\" name=\"{nameValue}\" type=\"checkbox\" value=\"{items[i].Id}\" {checkedValue} {disabledValue}>" +
                    $"<label class=\"checkbox{disabledClass} govuk-label govuk-checkboxes__label\" for=\"{idValue}\">" + items[i].Title +
                    $"</label>" +
                    $"</div>"
                );
            }
            stringBuilder.Append("</div>");

            return stringBuilder.ToString();
        }
    }
}
