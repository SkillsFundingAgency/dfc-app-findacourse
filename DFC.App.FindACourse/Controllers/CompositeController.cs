using Microsoft.AspNetCore.Mvc;

namespace DFC.App.FindACourse.Controllers
{
    public class CompositeController : Controller
    {
        [Route("head/{**data}")]
        public IActionResult Head()
        {
            return View();
        }
    }
}