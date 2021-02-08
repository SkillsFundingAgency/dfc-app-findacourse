using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DFC.App.FindACourse.Helpers
{
    public interface IViewHelper
    {
        Task<string> RenderViewAsync<TModel>(Controller controller, string viewName, TModel model, bool partial = false);
    }
}
