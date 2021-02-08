using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Helpers
{
    [ExcludeFromCodeCoverage]
    public class ViewHelper : IViewHelper
    {
        public async Task<string> RenderViewAsync<TModel>(Controller controller, string viewName, TModel model, bool isPartialView = false)
        {
            _ = controller ?? throw new ArgumentNullException(nameof(controller));

            controller.ViewData.Model = model;

            using var writer = new StringWriter();
            IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            ViewEngineResult viewResult = viewEngine.GetView(viewName, viewName, !isPartialView);

            if (!viewResult.Success)
            {
                return $"A view with the name {viewName} could not be found";
            }

            ViewContext viewContext = new ViewContext(
                controller.ControllerContext,
                viewResult.View,
                controller.ViewData,
                controller.TempData,
                writer,
                new HtmlHelperOptions());

            await viewResult.View.RenderAsync(viewContext).ConfigureAwait(false);

            return writer.GetStringBuilder().ToString();
        }
    }
}
