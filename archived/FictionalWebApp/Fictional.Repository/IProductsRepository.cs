using System;
using System.Threading.Tasks;
using Fictional.CommandResults;
using Fictional.Commands;

namespace Fictional.Repository
{
    public interface IProductsRepository
    {
        Task<BaseResult> SeedMockProducts();

        Task<GetProductsResult> GetProductsAsync();

        Task<GetProductResult> GetProductAsync(string productId);

        Task<CreateOrUpdateProductResult> CreateOrUpdateProductAsync(CreateOrUpdateProductCommand command);

        Task<BaseResult> DeleteProductAsync(string productId);
    }
}
