using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DailyScrumBagAPI.Models;

namespace DailyScrumBagAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("Home")]
    public class HomeController : Controller
    {
        [Route("")]      // Combines to define the route template "Home"
        [Route("Index")] // Combines to define the route template "Home/Index"
        [Route("/")]     // Doesn't combine, defines the route template ""
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet(Name = nameof(About))]
        [Route("About")]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [HttpGet(Name = nameof(Contact))]
        [Route("Contact")]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpGet(Name = nameof(Error))]
        [Route("Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
