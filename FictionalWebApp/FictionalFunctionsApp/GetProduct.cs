
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Fictional.Repository;

namespace FictionalFunctionsApp
{
    public static class GetProduct
    {
        [FunctionName("GetProduct")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("Looking up product in our records ...");

            string productId = req.Query["productId"];

            if (string.IsNullOrEmpty(productId))
                return new BadRequestObjectResult("Product Id is required");

            var productsRepository = new ProductsRepository();

            var result = await productsRepository.GetProductAsync(productId);

            if (result.IsSuccess)
                return new OkObjectResult(result);

            return new BadRequestObjectResult(result.Errors[0]);
        }
    }
}
