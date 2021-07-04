using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace blobproperties
{
    class Program
    {
        static string connstring = "DefaultEndpointsProtocol=https;AccountName=enbehstorage;AccountKey=pgSgc8psXmSTgwdIE+fsozrDkirh1YfbKcJWgn9fKRAdo54Dq4ugJ7PPENrK1sFrsAMZY7C3z8dq1U+hAZM9wA==;EndpointSuffix=core.windows.net";
        static string containerName = "data";
        static string filename = "sample.txt";
        static async Task Main(string[] args)
        {
            GetProperties();
            SetMetadata();
            GetMetadata();
            
            Console.WriteLine("Operation complete");
            Console.ReadKey();
        }

        static void GetProperties()
        {
            //it will make instance of the azure storage blob client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connstring);
            //it will make instance of the azure stroage blob container client
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            //it will make instnace of blob client by access the blob Name
            BlobClient blob = containerClient.GetBlobClient(filename);

            //it will get the blob properities
            BlobProperties properties=blob.GetProperties();
            //it will get the access tier and content length of blob
            Console.WriteLine("The Access tier of the blob is {0}",properties.AccessTier);
            Console.WriteLine("The Content Length of the blob is {0}", properties.ContentLength);           
        }

        static void GetMetadata()
        {
            //it will make instance of the azure storage blob client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connstring);
            //it will make instance of the azure stroage blob container client
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            //it will make instnace of blob client by access the blob Name
            BlobClient blob = containerClient.GetBlobClient(filename);
            //it will get the blob properities
            BlobProperties properties = blob.GetProperties();
            //it will get all meta data of the azure blob
            foreach(var metadata in properties.Metadata)
            {
                Console.WriteLine(metadata.Key.ToString());
                Console.WriteLine(metadata.Value.ToString());
            }
        }

        static void SetMetadata()
        {
            //it will set meta-data key and value pairs
            string p_key = "ApplicationType";
            string p_value = "Ecommerce";
            //it will make instance of the azure storage blob client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connstring);
            //it will make instance of the azure stroage blob container client
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            //it will make instnace of blob client by access the blob Name
            BlobClient blob = containerClient.GetBlobClient(filename);
         
            //it will set meta-data of the blob into azure contaienr
            IDictionary<string, string> obj = new Dictionary<string, string>();
            obj.Add(p_key, p_value);
            blob.SetMetadata(obj);
        }
    }
}
