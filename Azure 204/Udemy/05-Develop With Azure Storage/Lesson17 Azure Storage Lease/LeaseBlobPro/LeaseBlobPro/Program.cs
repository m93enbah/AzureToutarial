using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System;
using System.IO;

namespace LeaseBlobPro
{
    public class Program
    {
        private static string containerName = "data";
        private static string connectionString = "DefaultEndpointsProtocol=https;AccountName=enbehstorage;AccountKey=K5BGIjoGlvtsza+icbOXdAXGSseFE4neve/TYESguOPFHkG7DcaRXxyM1jZvyhNynvf1HCDrXsoqc6nJLhYOLg==;EndpointSuffix=core.windows.net";
        private static string blobName = "newfile1.txt";

        //the heirachy as below
        //1-BlobServiceClient : which used to access to storage account service and can know about Containers
        //2-ContainerClinet : which is need BlobServiceClient in order to access to containers
        //3-BlobClient : which is need to ContianerClient in order to access to Blob
        static void Main(string[] args)
        {
            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            UploadWithoutLease(blobClient);
            UploadWithLease(blobClient);


            Console.ReadLine();
        }

        public static void UploadWithoutLease(BlobClient blobClient)
        {
            var memory = new MemoryStream();

            blobClient.DownloadTo(memory);
            memory.Position = 0;

            var reader = new StreamReader(memory);
            Console.WriteLine(reader.ReadToEnd());

            var writer = new StreamWriter(memory);
            writer.Write("This is a change 01");
            writer.Flush();

            memory.Position = 0;
            blobClient.Upload(memory, true);

            Console.WriteLine("Change made");
        }

        public static void UploadWithLease(BlobClient blobClient) 
        {
            var memory = new MemoryStream();

            blobClient.DownloadTo(memory);
            memory.Position = 0;

            var reader = new StreamReader(memory);
            Console.WriteLine(reader.ReadToEnd());

            //we make blobLeaseClient instance and aquire lease id for duration
            var blobLeaseClient = blobClient.GetBlobLeaseClient();
            var lease = blobLeaseClient.Acquire(TimeSpan.FromSeconds(30));

            Console.WriteLine($"The Lease {lease.Value.LeaseId}");

            var writer = new StreamWriter(memory);
            writer.Write("This is a change 02");
            writer.Flush();

            var blobUplaodOptions = new BlobUploadOptions()
            {
                Conditions = new BlobRequestConditions()
                {
                    LeaseId = lease.Value.LeaseId
                }
            };

            memory.Position = 0;
            blobClient.Upload(memory, blobUplaodOptions);
            //release the lease id 
            blobLeaseClient.Release();

            Console.WriteLine("Change made");
        }

    }
}
