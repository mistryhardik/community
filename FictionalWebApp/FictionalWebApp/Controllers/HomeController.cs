using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FictionalWebApp.Models;
using Fictional.Repository;

namespace FictionalWebApp.Controllers
{
    public class HomeController : Controller
    {
        readonly IGreetingRepository _greetingRepository;

        public HomeController(IGreetingRepository greetingRepository)
        {
            _greetingRepository = greetingRepository;
        }

        public IActionResult Index()
        {
            var greeting = _greetingRepository.GetGreetingMessage(DateTime.Now.Hour);

            ViewBag.Greeting = greeting;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
