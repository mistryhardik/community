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
    public class OrderController : ControllerBase
    {
        [HttpPost("{productId}")]
        public ActionResult PlaceOrder([FromRoute] string productId)
        {
            // simulated api endpoint not for production use ;)
            Thread.Sleep(2000);
            
            // place your business logic here
            
            return new OkObjectResult($"{DateTime.UtcNow} :: Order placed: {productId}");
        }
    }
}
