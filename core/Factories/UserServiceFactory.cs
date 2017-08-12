using System.Threading.Tasks;
using core.Interfaces;
using core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace core.Factories
{
    public class UserServiceFactory : IUserServiceFactory
    {
        private IConfiguration Configuration { get; }
        public UserServiceFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // factory method to create UserService
        public UserService GetUserService()
        {
            var azureConnString = Configuration.GetConnectionString("AzureStorageConnectionString");
            
            // create storage account
            var storageAccount = CloudStorageAccount.Parse(azureConnString);
            
            // Create the table client.
            var tableClient = storageAccount.CreateCloudTableClient();
            
            // Get a reference to a table named "User"
            var userTable = tableClient.GetTableReference("User");

            // Create the User Table if it does not exist
            Task.Run(userTable.CreateIfNotExistsAsync).GetAwaiter().GetResult();
            
            // return userservice instance
            return new UserService(userTable);
        }
    }
}