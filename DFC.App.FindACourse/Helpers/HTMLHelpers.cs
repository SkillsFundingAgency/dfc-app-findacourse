using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Helpers
{
    public static class HTMLHelpers
    {
        public static IHtmlContent HtmlFormat(this IHtmlHelper htmlHelper, string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            return new HtmlString(content.Replace(System.Environment.NewLine, "<br/>"));
        }
    }
}
