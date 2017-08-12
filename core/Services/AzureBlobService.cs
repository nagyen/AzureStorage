using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace core.Services
{
    public abstract class AzureBlobService
    {
        protected CloudBlobContainer Container { get; }
        protected AzureBlobService(CloudBlobContainer container)
        {
            Container = container;
        }
	    
        // add or update a blob and return url
        protected async Task<string> UploadBlob(string blobName, string contentType, byte[] blobContent)
        {
            // Get a reference to a blob
            var blockBlob = Container.GetBlockBlobReference(blobName);

            // set content type
            blockBlob.Properties.ContentType = contentType;

            // Create or overwrite the blob with the blob contents
	        await blockBlob.UploadFromByteArrayAsync(blobContent, 0, blobContent.Length);

            // return url
            return blockBlob.Uri.ToString();
        }

        // get all blob urls
        protected async Task<List<string>> ListBlobs()
        {
	        var blobUrls = new List<string>();
	        
			BlobContinuationToken token = null;
			do
			{
				var resultSegment = await Container.ListBlobsSegmentedAsync(token);
				token = resultSegment.ContinuationToken;
				
				foreach (IListBlobItem item in resultSegment.Results)
				{
					blobUrls.Add(item.Uri.ToString());
				}
			} while (token != null);

            return blobUrls;
        }

        // get single blob url
        protected string GetBlobUrl(string blobName)
        {
			// Get a reference to a blob
			var blob = Container.GetBlockBlobReference(blobName);

            // retunr url
            return blob.Uri.ToString();
        }

        // download blob 
        protected async Task DownloadBlob(string blobName, string localFilePath)
        {
			// Get a reference to a blob
            var blockBlob = Container.GetBlockBlobReference(blobName);

			// Save the blob contents to a local file.
            using (var fileStream = System.IO.File.OpenWrite(localFilePath))
			{
				await blockBlob.DownloadToStreamAsync(fileStream);
			}
        }

        // delete blob
        protected async Task DeleteBlob(string blobName)
        {
			// Get a reference to a blob
            var blockBlob = Container.GetBlockBlobReference(blobName);

			// Delete the blob.
			await blockBlob.DeleteAsync();
        }
    }
}