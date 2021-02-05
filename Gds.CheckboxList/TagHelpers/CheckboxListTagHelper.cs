using GdsCheckboxList.Factory;
using GdsCheckboxList.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GdsCheckboxList.TagHelpers
{
    [ExcludeFromCodeCoverage]
    public class CheckBoxListTagHelper : TagHelper
    {
        public string Name { get; set; }

        public List<CheckBoxItem> Items { get; set; }

        public TemplateType Template { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Items == null)
            {
                throw new Exception("item property of checkbox-list cannot be null");
            }

            if (string.IsNullOrEmpty(Name))
            {
                throw new Exception("name property of checkbox-list cannot be null or empty");
            }

            output.TagName = string.Empty;
            var templateGenerator = TemplateGeneratorFactory.GetTemplateGenerator(Template);
            var template = templateGenerator.Generate(Name, Items);
            output.Content.SetHtmlContent(template);
        }
    }
}
