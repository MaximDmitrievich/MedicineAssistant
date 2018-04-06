using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebSendingASP.Models
{
    public class StorageConnecting
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobClient;
        private CloudBlobContainer _blobContainer;
        private CloudBlockBlob _blockBlobBlob;

        public CloudStorageAccount StorageAccount
        {
            private set
            {
                _storageAccount = value;
            }
            get
            {
                return _storageAccount;
            }
        }
        public CloudBlobClient BlobClient
        {
            private set
            {
                _blobClient = value;
            }
            get
            {
                return _blobClient;
            }
        }
        public CloudBlobContainer BlobContainer
        {
            private set
            {
                _blobContainer = value;
            }
            get
            {
                return _blobContainer;
            }
        }
        public CloudBlockBlob BlockBlobBlob
        {
            private set
            {
                _blockBlobBlob = value;
            }
            get
            {
                return _blockBlobBlob;
            }
        }

        public StorageConnecting()
        {
            _storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=medicineassistant;AccountKey=7RTxe5nrZb09eI+P1wvrZcAzcOTY4qWhzWGmvMsSntWw1YAXpEu0k4MoJmdpoBfUlCoFsX5y/Au5D94n63zUMg==;EndpointSuffix=core.windows.net");
            _blobClient = _storageAccount.CreateCloudBlobClient();
            _blobContainer = _blobClient.GetContainerReference("id1");
            //_blobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        public async Task SendFile(string json, string deviceid, DateTime date)
        {
            string name = $@"{date.Year}_{date.Month}_{date.Day}_{date.Hour}_{date.Minute}_{date.Second}.json";

            await _blobContainer.CreateIfNotExistsAsync();
            _blockBlobBlob = _blobContainer.GetBlockBlobReference($@"{deviceid}\\" + name);
            _blockBlobBlob.Properties.ContentType = "application/json";
            await _blockBlobBlob.UploadTextAsync(json);
        }

    }
}