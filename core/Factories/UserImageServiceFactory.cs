using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using core.Services;
using core.Interfaces;

namespace core.Factories
{
    public class UserImageServiceFactory : IUserImageServiceFactory
    {
        private IConfiguration Configuration { get;}
        public UserImageServiceFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

		public UserImageService GetUserImageService()
		{
			var azureConnString = Configuration.GetConnectionString("AzureStorageConnectionString");

			// create storage account
			var storageAccount = CloudStorageAccount.Parse(azureConnString);

			// Create the blob client.
			var blobClient = storageAccount.CreateCloudBlobClient();

			// Get a reference to a container
			var container = blobClient.GetContainerReference("user-images");

			// Create the user-images container if it does not exist
			Task.Run(container.CreateIfNotExistsAsync).GetAwaiter().GetResult();

			// set permissions to public
			Task.Run(() => container.SetPermissionsAsync(
					new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob }));

            return new UserImageService(container);

		}
    }
}
