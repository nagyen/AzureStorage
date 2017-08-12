using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace tests
{
    public class TableTests
    {
        // run all table test
        public static async Task Run()
        {
            var table = await CreateTable();
            await Add(table);
            await GetAll(table);
            await GetSingle(table);
            await DeleteSingle(table);
        }
        
        public static async Task<CloudTable> CreateTable()
        {
            var builder = new ConfigurationBuilder()
	             .AddJsonFile($"appsettings.json", false, true);
			var configuration = builder.Build();
			var azureConnString = configuration.GetConnectionString("AzureStorageConnectionString");
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(azureConnString);
            
            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Get a reference to a table named "peopleTable"
            CloudTable table = tableClient.GetTableReference("peopleTable");

            // Create the CloudTable if it does not exist
            await table.CreateIfNotExistsAsync();
            
            // return table
            return table;
        }

        public static async Task Add(CloudTable table)
        {
            // Create a new customer entity.
            CustomerEntity customer1 = new CustomerEntity("Harp", "Walter");
            customer1.Email = "Walter@contoso.com";
            customer1.PhoneNumber = "425-555-0101";

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer1);

            // Execute the insert operation.
            await table.ExecuteAsync(insertOperation);
        }

        public static async Task GetAll(CloudTable table)
        {
            // Construct the query operation for all customer entities 
            TableQuery<CustomerEntity> query = new TableQuery<CustomerEntity>();

            // Print the fields for each customer.
            TableContinuationToken token = null;
            do
            {
                TableQuerySegment<CustomerEntity> resultSegment = await table.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                foreach (CustomerEntity entity in resultSegment.Results)
                {
                    Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                        entity.Email, entity.PhoneNumber);
                }
            } while (token != null);
        }

        public static async Task GetSingle(CloudTable table)
        {
            // Create a retrieve operation that takes a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

            // Execute the retrieve operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result != null)
                Console.WriteLine(((CustomerEntity)retrievedResult.Result).PhoneNumber);
            else
                Console.WriteLine("The phone number could not be retrieved.");
        }

        public static async Task DeleteSingle(CloudTable table)
        {
            // Create a retrieve operation that expects a customer entity.
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Smith", "Ben");

            // Execute the operation.
            TableResult retrievedResult = await table.ExecuteAsync(retrieveOperation);

            // Assign the result to a CustomerEntity object.
            CustomerEntity deleteEntity = (CustomerEntity)retrievedResult.Result;

            // Create the Delete TableOperation and then execute it.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                await table.ExecuteAsync(deleteOperation);

                Console.WriteLine("Entity deleted.");
            }

            else
                Console.WriteLine("Couldn't delete the entity.");
        }
    }
    
    public class CustomerEntity : TableEntity
    {
        public CustomerEntity(string lastName, string firstName)
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }
        
        public CustomerEntity() { }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}