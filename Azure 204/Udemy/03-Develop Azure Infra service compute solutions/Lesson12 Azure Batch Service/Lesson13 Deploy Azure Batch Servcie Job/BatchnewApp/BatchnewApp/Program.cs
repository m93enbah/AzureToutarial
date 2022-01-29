using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchnewApp
{
    class Program
    {

        // Add the batch account crdentials here
        private const string demo_batchAccountName = "m93enbahbatch";
        private const string demo_batchAccountKey = "cMWeeESL3un+3y0zs+CZBzdi38JcE31vnHUhHMoiClu68juzeiOFruJseJHgiNqV+wv+QBXu4nTRzVJ+EDgFaQ==";
        private const string demo_batchAccountUrl = "https://m93enbahbatch.westeurope.batch.azure.com";

        // Here add the storage account details
        private const string demo_storageAccountName = "m93enbahstorage";
        private const string demo_storageAccountKey = "TaTrOo7x8JqM+dZTu2G0F8xEeObWj6ki+oSmup4dWHIgybTDd0BRCvLGBrZfBCz2ZrsEOf1FY7YBuF9tKu+UMQ==";


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
            BatchSharedKeyCredentials demo_sharedKeyCredentials = new BatchSharedKeyCredentials(demo_batchAccountUrl, demo_batchAccountName, demo_batchAccountKey);

            using (BatchClient demo_batchClient = BatchClient.Open(demo_sharedKeyCredentials))
            {

                // This method is used to create the pool
              //  await PoolCreation(demo_batchClient, PoolId);
                // This method is used to create the job
                //await JobCreation(demo_batchClient, jobID, PoolId);
                // This method is used to create the task
                await TaskCreation(demo_batchClient, jobID);
            }
        }

        private static async Task PoolCreation(BatchClient p_batchClient, string p_poolId)
        {

            Console.WriteLine("Creating the pool of virtual machines");
            try
            {
                var poolsLst = p_batchClient.PoolOperations.ListPools().ToList();

                var isExist = poolsLst.Where(x => x.Id == p_poolId).FirstOrDefault();

                if (isExist != null) 
                {
                    return;
                }

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
                demo_pool.ApplicationPackageReferences = new List<ApplicationPackageReference>{
                new ApplicationPackageReference{
                ApplicationId = demo_packageid,
                Version = demo_packageversion}};
                //it will apply and create the pool
                await demo_pool.CommitAsync();
            }
            catch (BatchException pool_error) { Console.WriteLine(pool_error.Message); }
        }

        private static async Task JobCreation(BatchClient p_batchClient, string p_jobId, string p_poolId)
        {
            //we get all jobs and check if the job exist or not to delete if exist and create new one
            var lst = p_batchClient.JobOperations.ListJobs().ToList();
            var isExist = lst.Where(x => x.Id == p_jobId).FirstOrDefault();
            if (isExist != null) { p_batchClient.JobOperations.DeleteJob(p_jobId); }

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
            //we define input and ouput containers
            Console.WriteLine("Creating the Task");

            string taskId = "demotask";
            string in_container_name = "inputs";
            string out_container_name = "outputs";
            string l_blobName = "1280.mp4";
            string outputfile = "audio.aac";


            string storageConnectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                                demo_storageAccountName, demo_storageAccountKey);

            CloudStorageAccount l_storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            //we create cloudblob client to access to blob containers
            CloudBlobClient l_blobClient = l_storageAccount.CreateCloudBlobClient();

            //we access to inputs container
            CloudBlobContainer in_container = l_blobClient.GetContainerReference(in_container_name);
            //we access to outputs container
            CloudBlobContainer out_container = l_blobClient.GetContainerReference(out_container_name);

            //we apply read , list permissions for inputs container
            SharedAccessBlobPolicy i_sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(2),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List
            };

            //we apply write permission for outputs container
            SharedAccessBlobPolicy o_sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(2),
                Permissions = SharedAccessBlobPermissions.Write 
            };

            //we generate SASS Token for input container and combine with the URL of the inputs container
            string in_sasToken = in_container.GetSharedAccessSignature(i_sasConstraints);
            string in_containerSasUrl = String.Format("{0}{1}", in_container.Uri, in_sasToken);

            //we generate SASS Token for input container and combine with the URL of the outputs container
            string out_sasToken = out_container.GetSharedAccessSignature(o_sasConstraints);
            string out_containerSasUrl = String.Format("{0}{1}", out_container.Uri, out_sasToken);

            //we declare resources file for the inputs container by set the input container url instead of 
            //write this command inputFile.Add(new ResourceFile(l_blobSasUrl, l_blobName));
            ResourceFile inputFile = ResourceFile.FromStorageContainerUrl(in_containerSasUrl);

            List<ResourceFile> file = new List<ResourceFile>();
            file.Add(inputFile);
            
            string appPath = String.Format("%AZ_BATCH_APP_PACKAGE_{0}#{1}%", demo_packageid, demo_packageversion);

            string taskCommandLine = String.Format("cmd /c {0}\\ffmpeg.exe -i {1} -vn -acodec copy audio.aac", appPath, l_blobName);

            CloudTask task = new CloudTask(taskId, taskCommandLine);
            task.ResourceFiles = file;

            // Setting the output file location and store the audio file inside the ouptus container
            List<OutputFile> outputFileList = new List<OutputFile>();
            OutputFileBlobContainerDestination outputContainer = new OutputFileBlobContainerDestination(out_containerSasUrl);
            OutputFile outputFile = new OutputFile(outputfile,
                                                   new OutputFileDestination(outputContainer),
                                                   new OutputFileUploadOptions(OutputFileUploadCondition.TaskSuccess));
            outputFileList.Add(outputFile);
            task.OutputFiles = outputFileList;


            await p_batchClient.JobOperations.AddTaskAsync(p_jobId, task);

        }

    }

}

