using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fictional.CommandResults;
using Fictional.Commands;
using Fictional.Models;
using Fictional.ViewModels;

namespace Fictional.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        public async Task<BaseResult> SeedMockProducts()
        {
            try
            {
                var result = await RepositoryManager.Instance.SeedMockProducts();

                return new BaseResult
                {
                    IsSuccess = result,
                    Message = "Products seed",
                    ResponseStatusCode  = System.Net.HttpStatusCode.Created
                };
            }
            catch(Exception ex)
            {
                return new BaseResult
                {
                    IsSuccess = true,
                    Errors = new List<string> { ex.Message },
                    ResponseStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public async Task<CreateOrUpdateProductResult> CreateOrUpdateProductAsync(CreateOrUpdateProductCommand command)
        {
            try
            {
                var result = await RepositoryManager.Instance.CreateOrUpdateProductAsync(new Product
                {
                    Id = string.IsNullOrEmpty(command.Id) ? Guid.NewGuid().ToString() : command.Id,
                    Name = command.Name,
                    Description = command.Description,
                    ImageUrl = command.ImageUrl,
                    RedirectUrl = command.RedirectUrl,
                    RowKey = Guid.NewGuid().ToString(),
                    PartitionKey = Guid.NewGuid().ToString()
                });

                return new CreateOrUpdateProductResult
                {
                    IsSuccess = result == null ? false : true,
                    ResponseStatusCode = result == null ? System.Net.HttpStatusCode.BadRequest : System.Net.HttpStatusCode.OK,
                    Product = result == null ? null : new ProductViewModel
                    {
                        Id = result.Id,
                        Name = result.Name,
                        Description = result.Description,
                        ImageUrl = result.ImageUrl,
                        RedirectUrl = result.RedirectUrl
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetProductResult> GetProductAsync(string productId)
        {
            try
            {
                var product = await RepositoryManager.Instance.GetProductAsync(productId);

                if (product == null)
                    return new GetProductResult
                    {
                        IsSuccess = true,
                        Message = "No product to get"
                    };

                return new GetProductResult
                {
                    IsSuccess = true,
                    Product = new ProductViewModel
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        ImageUrl = product.ImageUrl, 
                        RedirectUrl = product.RedirectUrl
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<GetProductsResult> GetProductsAsync()
        {
            try
            {
                var products = await RepositoryManager.Instance.GetProductsAsync();

                if (products == null)
                    return new GetProductsResult
                    {
                        IsSuccess = true,
                        Message = "No products in records"
                    };

                if(products.Count == 0)
                    return new GetProductsResult
                    {
                        IsSuccess = true,
                        Message = "No products in records"
                    };

                return new GetProductsResult
                {
                    IsSuccess = true,
                    Products = products.Select(p => new ProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        ImageUrl = p.ImageUrl,
                        RedirectUrl = p.RedirectUrl

                    }).ToList()

                };
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BaseResult> DeleteProductAsync(string productId)
        {
            try
            {
                var productToDelete = await RepositoryManager.Instance.GetProductAsync(productId);

                if (productToDelete == null)
                    return new BaseResult
                    {
                        IsSuccess = true,
                        Message = "No product to delete"
                    };

                var result = await RepositoryManager.Instance.DeleteProductAsync(productToDelete);

                return new BaseResult
                {
                    IsSuccess = result,
                    Message = result ? "Product deleted" : "Could not delete your product"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
