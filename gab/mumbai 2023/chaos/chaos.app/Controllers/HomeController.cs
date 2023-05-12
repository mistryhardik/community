using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using chaos.app.Models;

namespace chaos.app.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index([FromQuery] int chaos = 0)
    {
        ViewData["WelcomeMessage"] = $"Welcome from {_configuration.GetSection("APP_REGION").Value}";

        try
        {
            var limit = 9216;

            var stop = 0;
        
            while (stop < chaos)
            {
                var thread = new Thread(() => IncreaseMemory(limit));
                thread.Start();

                stop++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            RedirectPermanent("Error");
        }
        
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
    
    private static void IncreaseMemory(long limity)
    {
        var limit = limity;

        var list = new List<byte[]>();
        try
        {
            while(true)
            {
                list.Add(new byte[limit]); // Change the size here.
                Thread.Sleep(1000); // Change the wait time here.
            }
        }

        catch (Exception ex)
        {
            // do nothing
        }
    }
}