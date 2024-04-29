using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using heroapp.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace heroapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IExternalService _service;
        private string region = string.Empty;
        
        public HealthController(IExternalService service, IConfiguration config)
        {
            _service = service;
            region = config["APP_REGION"];
        }
        
        [HttpGet()]
        public ActionResult Check()
        {
            _service.RunDependencyService(region);
            
            return new OkObjectResult($"GAB 2024 from {region}");
        }
        
        [HttpGet("{timeout}")]
        public ActionResult SlowCheck([FromRoute] int timeout = 0)
        {
            // simulated api endpoint not for production use ;)
            Thread.Sleep(timeout);
            
            // place your business logic here
            
            return new OkObjectResult($"Slow GAB 2024 from {region} by {timeout}ms");
        }
    }
}
