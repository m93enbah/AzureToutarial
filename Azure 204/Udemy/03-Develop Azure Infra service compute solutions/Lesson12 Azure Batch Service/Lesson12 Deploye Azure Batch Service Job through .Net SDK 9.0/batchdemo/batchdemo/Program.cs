using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace batchdemo
{
    class Program
    {
        // Add the batch account crdentials here
        private const string demo_batchAccountName = "enbehbatchaccount";
        private const string demo_batchAccountKey = "lGSLGM1XnPCtOQrDrnqGotc0sC4kiYXLv73u8xx0EaMdiSZj/Z3/CgWU7t6ZozjHvm2jp6kFjISPnQYGtLXdjg==";
        private const string demo_batchAccountUrl = "https://enbehbatchaccount.westeurope.batch.azure.com";

        // Here add the storage account details
        private const string demo_storageAccountName = "enbehstorageaccount";
        private const string demo_storageAccountKey = "aBcpqRONCoYSb8bdfTLiIe3Q97vPvF9VgIIePI5J2z6pjkIVlUIWxj+HOi52o6cgXRiNVxVN4Hy7zF6l2iM1dw==";

        // These are general values required for the batch service
        private const string PoolId = "ffmpegpool";
        private const string jobID = "video_processor";
        private const string demo_packageid = "ffmpeg";
        private const string demo_packageversion = "3";


        static void Main(string[] args)
        {
            try
            {
                CoreAsync().Wait();
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Program complete");
                Console.ReadLine();
            }
        }

        private static async Task CoreAsync()
        {
            //we create batch account credential to batch url , batch account name , batch account access key
            BatchSharedKeyCredentials demo_sharedKeyCredentials = new BatchSharedKeyCredentials(demo_batchAccountUrl, demo_batchAccountName, demo_batchAccountKey);
            //we assign the batch credential to batch client to access to batch account
            using (BatchClient demo_batchClient = BatchClient.Open(demo_sharedKeyCredentials))
            {

                // This method is used to create the pool of virtual machines
                //await PoolCreation(demo_batchClient, PoolId);
                //await JobCreation(demo_batchClient, jobID, PoolId);
                await TaskCreation(demo_batchClient, jobID);
            }

        }
        private static async Task PoolCreation(BatchClient p_batchClient, string p_poolId)
        {
            Console.WriteLine("Creating the pool of virtual machines");
            try
            {
                //we define the image o.s 
                ImageReference demo_image = new ImageReference(
                            publisher: "MicrosoftWindowsServer",
                            offer: "WindowsServer",
                            sku: "2016-Datacenter",
                            version: "latest");

                VirtualMachineConfiguration demo_configuration =
                   new VirtualMachineConfiguration(
                       imageReference: demo_image,
                       nodeAgentSkuId: "batch.node.windows amd64");

                CloudPool demo_pool = null;

                //it will create 1 dedicated pool of type Standard_A1_V2 
                demo_pool = p_batchClient.PoolOperations.CreatePool(
                        poolId: p_poolId,
                        targetDedicatedComputeNodes: 1,
                        targetLowPriorityComputeNodes: 0,
                        virtualMachineSize: "STANDARD_D1_v2",
                        virtualMachineConfiguration: demo_configuration);

                //we assign the pool with package defined with id and version
                demo_pool.ApplicationPackageReferences = new List<ApplicationPackageReference>
                {
                    new ApplicationPackageReference
                    {
                    ApplicationId = demo_packageid,
                    Version = demo_packageversion
                    }
                };
                //it will apply and create the pool
                await demo_pool.CommitAsync();
            }
            catch (BatchException pool_error)
            {
                Console.WriteLine(pool_error.Message);
            }
        }

        private static async Task JobCreation(BatchClient p_batchClient, string p_jobId, string p_poolId)
        {
            //we get all jobs and check if the job exist or not to delete if exist and create new one
            var lst = p_batchClient.JobOperations.ListJobs().ToList();
            var isExist = lst.Where(x => x.Id == p_jobId).FirstOrDefault();
            if (isExist != null)
            {
                p_batchClient.JobOperations.DeleteJob(p_jobId);
            }

            Console.WriteLine("Creating the job");
            //it will create job with id and assign pool to it
            CloudJob demo_job = p_batchClient.JobOperations.CreateJob();
            demo_job.Id = p_jobId;
            demo_job.PoolInformation = new PoolInformation { PoolId = p_poolId };
            //it will apply the configuration
            await demo_job.CommitAsync();
        }

        private static async Task TaskCreation(BatchClient p_batchClient, string p_jobId)
        {
            Console.WriteLine("Creating the Task");
            string taskId = "demotask";
            string container_name = "inputs";
            string l_blobName = "1280.mp4";

            string storageConnectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                                demo_storageAccountName, demo_storageAccountKey);

            CloudStorageAccount l_storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            //it will create cloud blob client to access to blob 
            CloudBlobClient l_blobClient = l_storageAccount.CreateCloudBlobClient();

            //its reference from azure batch account,      
            List<ResourceFile> resourceFiles = new List<ResourceFile>();

            //it will get the container reference
            CloudBlobContainer l_container = l_blobClient.GetContainerReference(container_name);
            //it will get the blob block reference to get the URL and apply SASS 
            CloudBlockBlob l_blobData = l_container.GetBlockBlobReference(l_blobName);

            //it will define the credential for blob expire time and permission to applied for blob called 1280.mp4
            SharedAccessBlobPolicy l_sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(2),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List
            };

            //it will SASS (shared access token) and SASS URL
            string sasToken = l_container.GetSharedAccessSignature(l_sasConstraints);
            string l_blobSasUrl = String.Format("{0}{1}", l_blobData.Uri, sasToken);

            //whith the SASS we can add blob SASS URL to resource files to inform the azure batch service where to get files
            resourceFiles.Add(new ResourceFile(l_blobSasUrl, l_blobName));

            //we design the command for AZ_BATCH_APP_PACKAGE
            string appPath = String.Format("%AZ_BATCH_APP_PACKAGE_{0}#{1}%", demo_packageid, demo_packageversion);
            string taskCommandLine = String.Format("cmd /c {0}\\ffmpeg.exe -i {1} -vn -acodec copy audio.aac", appPath, l_blobName);

            //we define AZ Batch task with define task Id and command line and resoruce files
            CloudTask task = new CloudTask(taskId, taskCommandLine);
            task.ResourceFiles = resourceFiles;

            //it will add task for batch service
            await p_batchClient.JobOperations.AddTaskAsync(p_jobId, task);
        }
    }
}

