using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System;

namespace BlobConatinerPro
{
    public class Program
    {
        private static string containerName = "data";
        private static string connectionString = "DefaultEndpointsProtocol=https;AccountName=enbehstorage;AccountKey=K5BGIjoGlvtsza+icbOXdAXGSseFE4neve/TYESguOPFHkG7DcaRXxyM1jZvyhNynvf1HCDrXsoqc6nJLhYOLg==;EndpointSuffix=core.windows.net";
        private static string blobName = "newfile1.txt";
        private static string location = @"C:\Users\M.Enbeh.ext\Desktop\test\newFile.txt";
        private static string downloadLocation = @"C:\temp";

        //the heirachy as below
        //1-BlobServiceClient : which used to access to storage account service and can know about Containers
        //2-ContainerClinet : which is need BlobServiceClient in order to access to containers
        //3-BlobClient : which is need to ContianerClient in order to access to Blob
        static void Main(string[] args)
        {



            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);

            UploadBlob(containerClient);

            //ListContiners(serviceClient);

            //ListBlobs(containerClient);

            //DownloadBlob(containerClient);


            ReadBlob(containerClient);

            Console.ReadLine();
        }


        private static void ListContiners(BlobServiceClient serviceClient)
        {
            foreach (var item in serviceClient.GetBlobContainers())
                Console.WriteLine(item.Name);
        }


        private static void ListBlobs(BlobContainerClient containerClient) 
        {
            foreach (var item in containerClient.GetBlobs())
                Console.WriteLine(item.Name);
        }


        private static void UploadBlob(BlobContainerClient containerClient)
        {
            var blobClient = containerClient.GetBlobClient(blobName);
            blobClient.Upload(location);
        }


        private static void DownloadBlob(BlobContainerClient containerClient)
        {
            var blobClient = containerClient.GetBlobClient(blobName);
            blobClient.DownloadTo(downloadLocation);
        }


        public static void ReadBlob(BlobContainerClient containerClient) 
        {
            var blobUri = GenerateSAS(containerClient);

            var blobClient = new BlobClient(blobUri);

            blobClient.DownloadTo(location);
        }

        public static Uri GenerateSAS(BlobContainerClient containerClient) 
        {
            //we access to blobClient through ContainerClient
            var blobClient = containerClient.GetBlobClient(blobName);

            //we generate BlobSasBuilder with set containerName , blobName , and type of blob
            var builder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",      //to indicate that SASS URL for Blob
            };
            //we set permisions as read , list
            builder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.List);
            //we set expire on after 1 hour
            builder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(1);
            return blobClient.GenerateSasUri(builder);
        }
    }
}
