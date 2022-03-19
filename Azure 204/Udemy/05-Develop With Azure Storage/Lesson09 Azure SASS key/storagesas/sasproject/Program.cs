using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using System.IO;
using System.Threading.Tasks;

namespace sasprogram
{
    class Program
    {
        //define the storage account Name and account key and container name and blob name
        static string accountname = "sampleaccountname";
        static string accountkey = "qTC2MLYxRZkSyrXcJWmxjeslx+JV3NRUA0HP2MGwTjc05REE0D/FchxGRZH1NXaxDszS+cxvxoDk4VGhFMKSfw==";
        static string containerName = "samplecontainer";
        static string blobname = "newsample.txt";

        static async Task Main(string[] args)
        {
            Uri sharedacecss=GenerateSAS();
            Console.WriteLine(sharedacecss);
            ReadBlob(sharedacecss).Wait();
            Console.ReadKey();
        }

        static Uri GenerateSAS()
        {
            //we declare SASS credential with specify container and blob names with start and expiry dates 
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = containerName,
                BlobName = blobname,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(5) 
            };

            //provide SASS credential permission of Read operation
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            //provide provide storage key credential with pass account naem and account key
            var storageSharedKeyCredential = new StorageSharedKeyCredential(accountname,accountkey);

            //provide SASS Token by combine SASS credential and storage account Name and account key
            string sasToken = sasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();

            // Build the full URI to the Blob SAS from the SASS Token
            UriBuilder w_fullUri = new UriBuilder()
            {
                Scheme = "https",
                Host = string.Format("{0}.blob.core.windows.net", accountname),
                Path = string.Format("{0}/{1}", containerName, blobname),
                Query = sasToken
            };

            return w_fullUri.Uri;
        }

        //By pass SASS URL you can access to the BLOB directly
        static async Task ReadBlob(Uri p_SASUri)
        {
            BlobClient blobClient = new BlobClient(p_SASUri, null);

            BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();
            using (StreamReader reader = new StreamReader(blobDownloadInfo.Content, true))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
    }
}
