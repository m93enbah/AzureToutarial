using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Blob_v11
{
    class Program
    {
        static string _conenction_string = "DefaultEndpointsProtocol=https;AccountName=enbehstorage;AccountKey=pgSgc8psXmSTgwdIE+fsozrDkirh1YfbKcJWgn9fKRAdo54Dq4ugJ7PPENrK1sFrsAMZY7C3z8dq1U+hAZM9wA==;EndpointSuffix=core.windows.net";
        static string _container_name = "datav11";
        static string _filename = "sample.txt";
        static string _filelocation = "C:\\Work\\sample.txt";
        static void Main(string[] args)
        {
            CreateContainer().Wait();
            UploadBlob().Wait();
            ListBlobs().Wait();
            GetBlob().Wait();
            Console.WriteLine("Completed");
        }
        //it will create new container by pass the connection string of the Azure Container
        static async Task CreateContainer()
        {
            CloudStorageAccount _storageAccount;
            if (CloudStorageAccount.TryParse(_conenction_string, out _storageAccount))
            {
                CloudBlobClient _client = _storageAccount.CreateCloudBlobClient();
                CloudBlobContainer _container = _client.GetContainerReference(_container_name);
                await _container.CreateAsync();

            }
        }
        //it will upload BLOB into the Azure Storage Account by specify the file location
        static async Task UploadBlob()
        {
            CloudStorageAccount _storageAccount;
            if (CloudStorageAccount.TryParse(_conenction_string, out _storageAccount))
            {
                CloudBlobClient _client = _storageAccount.CreateCloudBlobClient();
                CloudBlobContainer _container = _client.GetContainerReference(_container_name);

                CloudBlockBlob _blob = _container.GetBlockBlobReference(_filename);
                await _blob.UploadFromFileAsync(_filelocation);
            }
        }
        //it will make list of blobs on the Azure Storage Account 
        static async Task ListBlobs()
        {
            CloudStorageAccount _storageAccount;
            if (CloudStorageAccount.TryParse(_conenction_string, out _storageAccount))
            {
                CloudBlobClient _client = _storageAccount.CreateCloudBlobClient();
                CloudBlobContainer _container = _client.GetContainerReference(_container_name);

                BlobContinuationToken _blobContinuationToken = null;
                do
                {
                    var _results = await _container.ListBlobsSegmentedAsync(null, _blobContinuationToken);

                    _blobContinuationToken = _results.ContinuationToken;

                    foreach (IListBlobItem _blob in _results.Results)
                    {
                        Console.WriteLine(_blob.Uri);
                    }
                }
                while (_blobContinuationToken != null);
            }
        }

        //it will download the blob and then read it
        static async Task GetBlob()
        {
            CloudStorageAccount _storageAccount;
            if (CloudStorageAccount.TryParse(_conenction_string, out _storageAccount))
            {
                CloudBlobClient _client = _storageAccount.CreateCloudBlobClient();
                CloudBlobContainer _container = _client.GetContainerReference(_container_name);

                CloudBlockBlob _blob = _container.GetBlockBlobReference(_filename);
                string str = await _blob.DownloadTextAsync();
                Console.WriteLine(str);
            }
        }
    }
}

