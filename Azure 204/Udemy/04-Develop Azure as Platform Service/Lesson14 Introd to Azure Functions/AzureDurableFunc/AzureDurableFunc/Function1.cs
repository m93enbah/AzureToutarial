using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureDurableFunc
{
    public static class Function1
    {
        //this orchestrator function call the activity function as below
        [FunctionName("Function1")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            // Replace "hello" with the name of your Durable Activity Function.
            outputs.Add(await context.CallActivityAsync<string>("SayHello", "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>("SayHello", "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>("ExecFunc", ("Mohammed Enbeh",23)));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return outputs;
        }

        //this activity function contians the our code we need to execute
        [FunctionName("SayHello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello {name}!";
        }

        //this activity function contians the our code we need to execute
        [FunctionName("ExecFunc")]
        public static string ExecFunc([ActivityTrigger] (string name,int age) person, ILogger log)
        {
            log.LogInformation($"Saying hello to {person.name} , your age is {person.age}.");
            return $"Hello {person.name} , {person.age}!";
        }

        //this is the starter function that call the orchestrator function 
        [FunctionName("Function1_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("Function1", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}