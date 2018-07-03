using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ADD THIS PART TO YOUR CODE
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace DocumentDBGettingStarted
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        // ADD THIS PART TO YOUR CODE
        private const string EndpointUrl = "https://tryoutcosmos.documents.azure.com:443/";
        private const string PrimaryKey = "vZ2KSurRhrwRhL4cb8LfU3OReN9Tt28CU4wYVqNVSgt8S53qvP0aZF8TMDI5t4zHdBXlnhNFtTv4QL5HIh4OOg==";
        private DocumentClient client;

        static void Main(string[] args)
        {
            // ADD THIS PART TO YOUR CODE
            try
            {
                Program p = new Program();
                p.GetStartedDemo().Wait();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        private async Task GetStartedDemo()
        {
            this.client = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);

            await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "OaaSDB" });

            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("OaaSDB"), new DocumentCollection { Id = "ActiveOrders" });
            // add partition key
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri("OaaSDB"), new DocumentCollection { Id = "HistoricOrders" });
            // add partition key
            // ADD THIS PART TO YOUR CODE
            Order andersenOrder = new Order
            {
                Id = "1",
                State = "Queued",
                Product = new Product { ProductId = "1", ProductName = "Omlette" },
                UserId = "UserBla",
                ShopId = "Italian",
                QueueOrderNumber = 1,
                Ingredients = new Ingredient[] { new Ingredient { IngredientId = "1", IngredientName = "Egg" } },
                OrderQueuedDateTime = DateTime.Now,
                OrderInProgressDateTime = DateTime.Now,
                OrderCompletedDateTime = DateTime.Now
            };

            await this.CreateFamilyDocumentIfNotExists("OaaSDB", "ActiveOrders", andersenOrder);


            // ADD THIS PART TO YOUR CODE
            this.ExecuteSimpleQuery("OaaSDB", "ActiveOrders");


            this.ExecuteSimpleQuery("OaaSDB", "ActiveOrders");


            // ADD THIS PART TO CODE
            // Clean up/delete the database
            await this.client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri("OaaSDB"));
        }

        // ADD THIS PART TO YOUR CODE
        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        // ADD THIS PART TO YOUR CODE
        public class Order
        {
            [JsonProperty(PropertyName = "id")]
            public String Id { get; set; }
            public string State { get; set; }
            public Product Product { get; set; }
            public string UserId { get; set; }
            public string ShopId { get; set; }
            public int QueueOrderNumber { get; set; }
            public Ingredient[] Ingredients { get; set; }
            public DateTime OrderQueuedDateTime { get; set; }
            public DateTime OrderInProgressDateTime { get; set; }
            public DateTime OrderCompletedDateTime { get; set; }
            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        public class Product
        {
            public string ProductId { get; set; }
            public string ProductName { get; set; }
        }


        public class Ingredient
        {
            public string IngredientId { get; set; }
            public string IngredientName { get; set; }
        }

        // ADD THIS PART TO YOUR CODE
        private async Task CreateFamilyDocumentIfNotExists(string databaseName, string collectionName, Order order)
        {
            try
            {
                await this.client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, order.Id));
                this.WriteToConsoleAndPromptToContinue("Found {0}", order.Id);
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), order);
                    this.WriteToConsoleAndPromptToContinue("Created Order {0}", order.Id);
                }
                else
                {
                    throw;
                }
            }
        }

        // ADD THIS PART TO YOUR CODE
        private void ExecuteSimpleQuery(string databaseName, string collectionName)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1 };

            // Here we find the Andersen family via its LastName
            IQueryable<Order> orderQuery = this.client.CreateDocumentQuery<Order>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                    .Where(f => f.Id == "1");

            // The query is executed synchronously here, but can also be executed asynchronously via the IDocumentQuery<T> interface
            Console.WriteLine("Running LINQ query...");
            foreach (Order order in orderQuery)
            {
                Console.WriteLine("\tRead {0}", order);
            }

            // Now execute the same query via direct SQL
            IQueryable<Order> orderQueryInSql = this.client.CreateDocumentQuery<Order>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                    "SELECT * FROM ActiveOrders WHERE Id = 1",
                    queryOptions);

            Console.WriteLine("Running direct SQL query...");
            foreach (Order order in orderQueryInSql)
            {
                Console.WriteLine("\tRead {0}", order);
            }

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        /*
         private async Task ReplaceOrderDocument(string databaseName, string collectionName, int Id, Order updatedOrder)
               {
                   await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, Id), updatedOrder);
                   this.WriteToConsoleAndPromptToContinue("Replaced Order {0}", Id);
               }

               // ADD THIS PART TO YOUR CODE
               private async Task DeleteOrderDocument(string databaseName, string collectionName, string documentName)
               {
                   await this.client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, documentName));
                   Console.WriteLine("Deleted Order {0}", documentName);
               }
           }
           */
    }
}
