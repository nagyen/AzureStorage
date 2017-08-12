using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace tests
{
    public class BlobTests
    {
        public static async Task Run()
        {
            var test = new BlobTests();
            var container = await test.CreateContainer();
            await test.UploadBlob(container);
            await test.ListBlobs(container);
            await test.DownloadBlob(container);
            //await test.DeleteBlob(container);
        }

        public async Task<CloudBlobContainer> CreateContainer()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false, true);
            var configuration = builder.Build();
            
            var azureConnString = configuration.GetConnectionString("AzureStorageConnectionString");
            
            // create storage account
            var storageAccount = CloudStorageAccount.Parse(azureConnString);
            
            // Create the blob client.
            var blobClient = storageAccount.CreateCloudBlobClient();
            
            // Get a reference to a container
            var container = blobClient.GetContainerReference("user-images");

            // Create the User Table if it does not exist
            await container.CreateIfNotExistsAsync();
            
            // set permissions to public
            await container.SetPermissionsAsync(
                new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

            return container;
        }

        public async Task UploadBlob(CloudBlobContainer container)
        {
            // Get a reference to a blob named "myblob".
            // note: file extension is not necessary if proper content-type is set like below
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

            // set content type to image
            blockBlob.Properties.ContentType = "image/jpg";
			// todo: calling save properties throws error
			//await blockBlob.SetPropertiesAsync();

			// Create or overwrite the blob with the contents of a local file
			var file = System.IO.File.OpenRead(@"demo-image.jpg");
            using (var fileStream = file)
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }
            
            Console.WriteLine("Uploaded image.");
        }

        public async Task ListBlobs(CloudBlobContainer container)
        {
			BlobContinuationToken token = null;
			do
			{
				BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
				token = resultSegment.ContinuationToken;

				foreach (IListBlobItem item in resultSegment.Results)
				{
					if (item.GetType() == typeof(CloudBlockBlob))
					{
						CloudBlockBlob blob = (CloudBlockBlob)item;
						Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
					}

					else if (item.GetType() == typeof(CloudPageBlob))
					{
						CloudPageBlob pageBlob = (CloudPageBlob)item;

						Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);
					}

					else if (item.GetType() == typeof(CloudBlobDirectory))
					{
						CloudBlobDirectory directory = (CloudBlobDirectory)item;

						Console.WriteLine("Directory: {0}", directory.Uri);
					}
				}
			} while (token != null);
        }

        public async Task DownloadBlob(CloudBlobContainer container)
        {
			// Get a reference to a blob named "myblob".
			CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

			// Save the blob contents to a file named "myblob-download".
			using (var fileStream = System.IO.File.OpenWrite(@"/Users/NagarjunaYendluri/Downloads/myblob-download.jpg"))
			{
				await blockBlob.DownloadToStreamAsync(fileStream);
			}

            Console.WriteLine("Downloaded blob");
        }

        public async Task DeleteBlob(CloudBlobContainer container)
        {
			// Get a reference to a blob named "myblob".
			CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

			// Delete the blob.
			await blockBlob.DeleteAsync();

            Console.WriteLine("Deleted blob");
        }
    }
}