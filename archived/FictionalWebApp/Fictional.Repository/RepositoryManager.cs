using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fictional.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Fictional.Repository
{
    public sealed class RepositoryManager
    {
        string TableName;
        string StorgeConnectionString;

        CloudTable table;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static RepositoryManager()
        {

        }

        private RepositoryManager()
        {
            TableName = "Products";
            StorgeConnectionString = "DefaultEndpointsProtocol=https;AccountName=appmatticrgdiag395;AccountKey=jFenYFVZWZ0gp2hj9x3w3yeCnj2Y0XeFC0FJT7m0/iX2dNJi0U8HjYFsFGOaZdcPDeNRHho3Ww5D7NfpEAP+6Q==;EndpointSuffix=core.windows.net";
        }

        public static RepositoryManager Instance { get; } = new RepositoryManager();

        /// <summary>
        /// Create the table and seeds the mock products.
        /// </summary>
        /// <returns>Status</returns>
        public async Task<bool> SeedMockProducts()
        {
            try
            {
                // create a products table, if exists, do nothing
                await CreateTableAsync();

                // fetch and delete exisitng products from table
                await ClearProductsAsync();

                var mockProducts = new List<Product>
                {
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Azure DevOps",
                        Description = "Plan smarter, collaborate better, and ship faster with a set of modern dev services.",
                        ImageUrl = "https://pbs.twimg.com/profile_images/1013370642417225728/BpqlqOrE.jpg",
                        RedirectUrl = "https://azure.microsoft.com/en-us/services/devops/?nav=min"
                    },
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Azure Boards",
                        Description = "Deliver value to your users faster using proven agile tools to plan, track, and discuss work across your teams.",
                        ImageUrl = "https://azurecomcdn.azureedge.net/cvt-eab04c07cb5f4c96549e8a1762229777bd601edec3b7980b03f2059b273d1ce2/images/shared/services/devops/boards-icon-80.png",
                        RedirectUrl = "https://azure.microsoft.com/en-us/services/devops/boards/?nav=min"
                    },
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Azure Pipelines",
                        Description = "Build, test, and deploy with CI/CD that works with any language, platform, and cloud. Connect to GitHub or any other Git provider and deploy continuously.",
                        ImageUrl = "https://azurecomcdn.azureedge.net/cvt-cb8eaff85f5782255dbd138587226ea4cfa26723fa3ad1555296c4de86e330a4/images/shared/services/devops/pipelines-icon-80.png",
                        RedirectUrl = "https://azure.microsoft.com/en-us/services/devops/pipelines/?nav=min"
                    },
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Azure Repos",
                        Description = "Get unlimited, cloud-hosted private Git repos and collaborate to build better code with pull requests and advanced file management.",
                        ImageUrl = "https://azurecomcdn.azureedge.net/cvt-d9921f3761c04cbd7e7a49c90b79631331e039c609afaf91bbddb82401245275/images/shared/services/devops/repos-icon-80.png",
                        RedirectUrl = "https://azure.microsoft.com/en-us/services/devops/repos/?nav=min"
                    },
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Azure Test Plans",
                        Description = "Test and ship with confidence using manual and exploratory testing tools.",
                        ImageUrl = "https://azurecomcdn.azureedge.net/cvt-4fd61d47055b51282cb78091bbad637e692ebb0152537ac7a0e1e72b82bb588d/images/shared/services/devops/test-plans-icon-80.png",
                        RedirectUrl = "https://azure.microsoft.com/en-us/services/devops/test-plans/?nav=min"
                    },
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Azure Artifacts",
                        Description = "Create, host, and share packages with your team, and add artifacts to your CI/CD pipelines with a single click.",
                        ImageUrl = "https://azurecomcdn.azureedge.net/cvt-b0f0acba20d774420fe84ab8b306d4d87c5cf6d366a3eab45454975e07f46df9/images/shared/services/devops/artifacts-icon-72.png",
                        RedirectUrl = "https://azure.microsoft.com/en-us/services/devops/artifacts/?nav=min"
                    },
                    new Product
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Azure Artifacts",
                        Description = "Access extensions from Slack to SonarCloud to 1,000 other apps and services—built by the community.",
                        ImageUrl = "https://pbs.twimg.com/profile_images/1013370642417225728/BpqlqOrE.jpg",
                        RedirectUrl = "https://marketplace.visualstudio.com/azuredevops"
                    }
                };
            
                foreach (var item in mockProducts)
                {
                    item.PartitionKey = Guid.NewGuid().ToString();
                    item.RowKey = Guid.NewGuid().ToString();

                    await CreateOrUpdateProductAsync(item);
                }

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        CloudTable GetTable()
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(StorgeConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create a table client for interacting with the table service 
            table = tableClient.GetTableReference(TableName);

            return table;
        }

        /// <summary>
        /// Create a table for the sample application to process messages in. 
        /// </summary>
        /// <returns>A CloudTable object</returns>
        public async Task<CloudTable> CreateTableAsync()
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(StorgeConnectionString);

            // Create a table client for interacting with the table service
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            Console.WriteLine("1. Create a Table for the demo");

            // Create a table client for interacting with the table service 
            table = tableClient.GetTableReference(TableName);
            try
            {
                if (await table.CreateIfNotExistsAsync())
                {
                    Console.WriteLine("Created Table named: {0}", TableName);
                }
                else
                {
                    Console.WriteLine("Table {0} already exists", TableName);
                }
            }
            catch (StorageException)
            {
                Console.WriteLine("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return table;
        }

        /// <summary>
        /// Create or update product in the table 
        /// </summary>
        /// <param name="product">The product entity</param>
        public async Task<Product> CreateOrUpdateProductAsync(Product product)
        {
            try
            {
                if (table == null)
                    table = GetTable();

                var operation = TableOperation.InsertOrReplace(product);

                var result = await table.ExecuteAsync(operation);

                var productCreated = result.Result as Product;

                return productCreated;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get all products from the table
        /// </summary>
        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                if (table == null)
                    table = GetTable();

                var query = new TableQuery<Product>();

                var result = await table.ExecuteQuerySegmentedAsync(query, null);

                return result.Results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get Product by Id from the table 
        /// </summary>
        /// <param name="productId">Id of the product to get</param>
        public async Task<Product> GetProductAsync(string productId)
        {
            try
            {
                if (table == null)
                    table = GetTable();

                var filter = TableQuery.GenerateFilterCondition("Id", QueryComparisons.Equal, productId);

                var query = new TableQuery<Product>().Where(filter);

                var result = await table.ExecuteQuerySegmentedAsync(query, null);

                if (result.Results.Count == 0)
                    return null;

                return result.Results[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Delete a product from the table
        /// </summary>
        /// <param name="product">The product entity</param>
        public async Task<bool> DeleteProductAsync(Product product)
        {
            try
            {
                if (table == null)
                    table = GetTable();

                var operation = TableOperation.Delete(product);

                var result = await table.ExecuteAsync(operation);

                if (result.HttpStatusCode == 204)
                    return true;

                throw new Exception("Could not delete the product");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> ClearProductsAsync()
        {
            try
            {
                if (table == null)
                    table = GetTable();

                var products = await GetProductsAsync();

                foreach (var item in products)
                {
                    var operation = TableOperation.Delete(item);

                    var result = await table.ExecuteAsync(operation);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Validate the connection string information in app.config and throws an exception if it looks like 
        /// the user hasn't updated this to valid values. 
        /// </summary>
        /// <param name="storageConnectionString">Connection string for the storage service or the emulator</param>
        /// <returns>CloudStorageAccount object</returns>
        CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the application.");
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
    }
}
