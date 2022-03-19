using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace blobprogram
{
    //we using Azure Storage Blob Nuget package
    class Program
    {
        static string storageconnstring = "DefaultEndpointsProtocol=https;AccountName=sampleaccountname;AccountKey=qTC2MLYxRZkSyrXcJWmxjeslx+JV3NRUA0HP2MGwTjc05REE0D/FchxGRZH1NXaxDszS+cxvxoDk4VGhFMKSfw==;EndpointSuffix=core.windows.net";
        static string containerName = "samplecontainertest";
        static string filename = "sample.txt";
        static string filepath= @"C:\Users\M.Enbeh.ext\Desktop\downloadSample.png";
        static string downloadpath = "C:\\Work\\sample.txt";
        static async Task Main(string[] args)
        {
            Container().Wait();
            CreateBlob().Wait();
            GetBlobs().Wait();
            //GetBlob().Wait();
            Console.WriteLine("Complete");
            Console.ReadKey();
        }

        static async Task Container()
        {
            //it create blob servcie client by setting connection string to allow permision 
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);
            //it will create storage blob async by setting container name
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
        }

        static async Task CreateBlob()
        {
            //it create blob servcie client by setting connection string to allow permision 
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);
            //it will call the container client to access the container name 
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            //it will get the blob client to access to file name to upload to the container 
            BlobClient blobClient = containerClient.GetBlobClient(filename);
            //it will read file path as FileStream
            using (FileStream uploadFileStream = File.OpenRead(filepath))
            {
                //it will uplaod the file stream async
                await blobClient.UploadAsync(uploadFileStream, true);
                uploadFileStream.Close();
            }
        }


        static async Task GetBlobs()
        {
            //it create blob servcie client by setting connection string to allow permision 
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);
            //it will call the container client to access the container name 
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            //it will get the blobs from the Azure storage > container 
            await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }
        }

        static async Task GetBlob()
        {
            //it create blob servcie client by setting connection string to allow permision 
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageconnstring);
            //it will call the container client to access the container name 
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            //it will make client instance to filename under container
            BlobClient blob = containerClient.GetBlobClient(filename);
            //it will apply downlaod async 
            BlobDownloadInfo blobdata = await blob.DownloadAsync();

            //it will apply downlaod file to specific physical location
            using (FileStream downloadFileStream = File.OpenWrite(downloadpath))
            {
                await blobdata.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
            }


            // Read the new file downloaded line by line
            using (FileStream downloadFileStream = File.OpenRead(downloadpath))
            {
                using var strreader = new StreamReader(downloadFileStream);
                string line;
                while ((line = strreader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }

        }
    }
}
