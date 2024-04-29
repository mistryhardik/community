using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using heroapp.Models;
using heroapp.Repository;

namespace heroapp.Controllers;

public class HomeController : Controller
{
    private readonly IExternalService _service;
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(IExternalService service, ILogger<HomeController> logger, IConfiguration configuration)
    {
        _service = service;
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index([FromQuery] int chaos = 0)
    {
        ViewData["WelcomeMessage"] = $"Welcome from {_configuration.GetSection("APP_REGION").Value}";

        try
        {
            _service.RunDependencyService(_configuration.GetSection("APP_REGION").Value);
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