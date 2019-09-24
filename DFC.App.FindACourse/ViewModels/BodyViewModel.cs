using Microsoft.AspNetCore.Html;

namespace DFC.App.FindACourse.ViewModels
{
    public class BodyViewModel
    {
        public HtmlString Content { get; set; } = new HtmlString("Unknown Find a course content");
    }
}