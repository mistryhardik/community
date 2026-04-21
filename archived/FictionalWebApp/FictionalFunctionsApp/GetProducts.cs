using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Fictional.Repository;

namespace FictionalFunctionsApp
{
    public static class GetProducts
    {
        [FunctionName("GetProducts")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("Looking up products in our records ...");

            var productsRepository = new ProductsRepository();

            var result = await productsRepository.GetProductsAsync();

            if (result.IsSuccess)
                return new OkObjectResult(result);

            return new BadRequestObjectResult(result.Errors[0]);
        }
    }
}
