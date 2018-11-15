using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Fictional.Repository;

namespace FictionalFunctionsApp
{
    public static class DeleteProduct
    {
        [FunctionName("DeleteProduct")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("Deleting the product ...");

            string productId = req.Query["productId"];

            if (string.IsNullOrEmpty(productId))
                return new BadRequestObjectResult("Product Id is required");

            var productsRepository = new ProductsRepository();

            var result = await productsRepository.DeleteProductAsync(productId);

            if (result.IsSuccess)
                return new NoContentResult();

            return new BadRequestObjectResult(result.Errors[0]);
        }
    }
}
