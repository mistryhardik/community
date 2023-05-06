using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace gab.backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public ActionResult Check()
        {
            // simulated api endpoint not for production use ;)
            Thread.Sleep(2000);
            
            // place your business logic here
            
            return new OkResult();
        }
    }
}
