using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fictional.CommandResults;
using Fictional.Commands;
using Fictional.Repository;
using Microsoft.AspNetCore.Mvc;

// NOTE: Run the Seed action first, to create the table and seed mock prodcuts

namespace FictionalApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        IProductsRepository productsRepository;

        public ProductsController()
        {
            if (productsRepository == null)
                productsRepository = new ProductsRepository();
        }

        /// <summary>
        /// Seed mock prodcuts
        /// </summary>
        /// <returns>Status</returns>
        [HttpGet("seed")]
        public async Task<BaseResult> Seed()
        {
            return await productsRepository.SeedMockProducts();
        }

        /// <summary>
        /// Get all products from the table
        /// </summary>
        /// <returns>List of products</returns>
        [HttpGet]
        public async Task<GetProductsResult> Get()
        {
            return await productsRepository.GetProductsAsync();
        }

        /// <summary>
        /// Get a product by Id
        /// </summary>
        /// <returns>A product</returns>
        /// <param name="productId">Product identifier.</param>
        [HttpGet("{productId}")]
        public async Task<GetProductResult> Get([FromRoute] string productId)
        {
            return await productsRepository.GetProductAsync(productId);
        }

        /// <summary>
        /// Create or update a product
        /// </summary>
        /// <returns>Status of the operation</returns>
        /// <param name="command">Command to create or update a product</param>
        [HttpPost]
        public async Task<CreateOrUpdateProductResult> CreateOrUpdate([FromBody] CreateOrUpdateProductCommand command)
        {
            var result = await productsRepository.CreateOrUpdateProductAsync(command);

            return result;
        }

        /// <summary>
        /// Delete the specified productId.
        /// </summary>
        /// <returns>Status of operation</returns>
        /// <param name="productId">Id of product to delete</param>
        [HttpDelete("{productId}")]
        public async Task<BaseResult> Delete([FromRoute] string productId)
        {
            var result = await productsRepository.DeleteProductAsync(productId);

            return result;
        }
    }
}
