﻿using GdsCheckboxList.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GdsCheckboxList.Templates
{
    /// <summary>
    /// Basic template generator class used to generate basic HTML markup
    /// <inpu>
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class BasicTemplateGenerator : ITemplateGenerator
    {
        public string Generate(string name, List<CheckBoxItem> items)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < items.Count; i++)
            {
                string checkedValue = items[i].IsChecked ? "checked" : string.Empty;
                string disabledValue = items[i].IsDisabled ? "disabled" : string.Empty;
                string nameValue = name;
                string idValue = name + $"[{i}]";
                string disabledClass = items[i].IsDisabled ? " disabled" : string.Empty;

                stringBuilder.Append(
                    $"<label class=\"checkbox{disabledClass}\" for=\"{idValue}\">" +
                        $"<input name=\"{nameValue}\" value=\"{items[i].Id}\" type =\"checkbox\" id=\"{idValue}\" {checkedValue} {disabledValue}>{items[i].Title}" +
                    $"</label>" +
                    $"<br>"
                );
            }

            return stringBuilder.ToString();
        }
    }
}
