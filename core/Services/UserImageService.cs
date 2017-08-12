using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace core.Services
{
    public class UserImageService : AzureBlobService
    {
        public UserImageService(CloudBlobContainer container): base(container)
        {
        }

        // add user image and return url
        public async Task<string> AddImage(string imageId, byte[] imageBytes)
        {
            var blobUrl = await UploadBlob(imageId, "image/jpg", imageBytes);

            return blobUrl;
        }
    }
}
