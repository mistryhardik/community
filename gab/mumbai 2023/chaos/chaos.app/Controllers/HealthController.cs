using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chaos.app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("{timeout}")]
        public ActionResult Check([FromRoute] int timeout = 0)
        {
            // simulated api endpoint not for production use ;)
            Thread.Sleep(timeout);
            
            // place your business logic here
            
            return new OkResult();
        }
    }
}
