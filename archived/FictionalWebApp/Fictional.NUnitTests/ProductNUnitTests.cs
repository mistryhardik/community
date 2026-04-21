using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fictional.Repository;
using NUnit.Framework;

namespace Fictional.NUnitTests
{
    [TestFixture]
    public class ProductNUnitTests
    {
        IProductsRepository productsRepository;
        string productId;

        [SetUp]
        public void Setup()
        {
            productsRepository = new ProductsRepository();
        }

        [TestCase]
        [Order(1)]
        public async Task Get_Products_Should_Return_Greater_Than_Zero_Products()
        {
            //seed mock products
            await productsRepository.SeedMockProducts();

            var result = await productsRepository.GetProductsAsync();

            Assert.AreEqual(result.IsSuccess, true);

            Assert.Greater(result.Products.Count, 0);

            productId = result.Products[0].Id;
        }

        [TestCase]
        [Order(2)]
        public async Task Get_Product_Should_Return_Product_Details()
        {
            var result = await productsRepository.GetProductAsync(productId);

            Assert.AreEqual(result.IsSuccess, true);

            Assert.NotNull(result.Product);

            // Assert for one or more properties of the object
            // we assert only if the ID is not null in this case
            Assert.IsNotNull(result.Product.Id);
        }

        [TestCase]
        [Order(3)]
        public async Task Create_Product_Should_Return_Product_Created()
        {
            var result = await productsRepository.CreateOrUpdateProductAsync(new Commands.CreateOrUpdateProductCommand
            {
                Name = "NUnit",
                Description = "Most popular unit test framework for .NET.",
                ImageUrl = "https://yourwebsite/image.png",
                RedirectUrl = "https://nunit.org/"
            });

            Assert.AreEqual(result.IsSuccess, true);

            Assert.NotNull(result.Product);

            // Assert for one or more properties of the object
            // we assert only if the ID is not null in this case
            Assert.IsNotNull(result.Product.Id);
        }

        [TestCase]
        [Order(4)]
        public async Task Delete_Product_Should_Return_Success()
        {
            var testProductIds = await PopulateProductsForTest();

            if(testProductIds.Count > 0)
            {
                // TODO: Replace with your product id to delete
                var result = await productsRepository.DeleteProductAsync(testProductIds[0]);

                Assert.AreEqual(result.IsSuccess, true);
            }
            else
                Assert.Pass("No products to delete");
        }

        async Task<List<string>> PopulateProductsForTest()
        {
            var commands = new List<Commands.CreateOrUpdateProductCommand>
            {
                new Commands.CreateOrUpdateProductCommand
                {
                    Name = "NUnit",
                    Description = "Most popular unit test framework for .NET.",
                    ImageUrl = "https://yourwebsite/image.png",
                    RedirectUrl = "https://nunit.org/"
                },
                new Commands.CreateOrUpdateProductCommand
                {
                    Name = "NUnit",
                    Description = "Most popular unit test framework for .NET.",
                    ImageUrl = "https://yourwebsite/image.png",
                    RedirectUrl = "https://nunit.org/"
                }
            };

            var ids = new List<string>();

            foreach (var command in commands)
            {
                var result = await productsRepository.CreateOrUpdateProductAsync(command);

                if (result.IsSuccess)
                    ids.Add(result.Product.Id);
            }

            return ids;
        }
    }
}
