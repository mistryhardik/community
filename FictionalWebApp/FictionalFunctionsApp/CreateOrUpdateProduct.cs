using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Fictional.Commands;
using Fictional.Repository;

namespace FictionalFunctionsApp
{
    public static class CreateOrUpdateProduct
    {
        [FunctionName("CreateOrUpdateProduct")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("Processing your request...");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var command = JsonConvert.DeserializeObject<CreateOrUpdateProductCommand>(requestBody);

            if (command == null)
                return new BadRequestObjectResult("Product information is required");

            var productRepository = new ProductsRepository();

            var result = await productRepository.CreateOrUpdateProductAsync(command);

            if (result.IsSuccess)
                return new OkObjectResult("Prodcut processed");
            else
                return new BadRequestObjectResult("An error occured: " + result.Errors[0]);
        }
    }
}
