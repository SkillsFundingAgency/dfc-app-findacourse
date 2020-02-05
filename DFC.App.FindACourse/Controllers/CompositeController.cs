using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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