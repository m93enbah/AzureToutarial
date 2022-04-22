using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageTable
{
    public class AzureTableStorage
    {

        private static readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=enbehaccount;AccountKey=TG9kCmt+EUHxOF5f9CgegRmYvYN05eDYdG/5aTHyGlYVHC0zXvdgeC/331zwjXN+YO1tEfiHRS4seZao7Y2CMA==;EndpointSuffix=core.windows.net";
        private static readonly string tableName = "Customers";
        private static readonly string rowKey = "CA-03";
        private static readonly string partitionKey = "Jordan";

        private static CloudTable InitializeTableClient() 
        {
            var account = CloudStorageAccount.Parse(connectionString);
            var tableClient = account.CreateCloudTableClient();
            return  tableClient.GetTableReference(tableName);
        }

        public async static Task InitializeStorageAccount()
        {
           var tableClinet = InitializeTableClient();
            await tableClinet.CreateIfNotExistsAsync();
        }

        public async static Task AddEntity()
        {
            var tableClinet = InitializeTableClient();

            var cust = new Customers("ali ahmad", 33, "Male", "Jordan", "CA-03");
            var operation = TableOperation.Insert(cust);
            var tableResult = await tableClinet.ExecuteAsync(operation);
            var result = tableResult.Result as Customers;
            Console.WriteLine($"Customer Info : User Name : {result.Name} , Age : {result.Age} , Country :");
        }

        //all the entities must have the same partition key
        public async static Task AddEntities()
        {
            var tableRef = InitializeTableClient();

            var custs = new List<Customers>(){
             new Customers("ali ahmad", 33, "Male", "Jordan", "CA-03"),
            new Customers("ali ahmad", 34, "Male", "Jordan", "CA-04"),
             new Customers("ali ahmad", 35, "Male", "Jordan", "CA-05"),
             new Customers("ali ahmad", 36, "Male", "Jordan", "CA-06"),
           };

            var batchOperation = new TableBatchOperation();
            foreach (var cust in custs)
                batchOperation.Insert(cust);

            var tableResult = await tableRef.ExecuteBatchAsync(batchOperation);
            for(var idx = 0; idx < tableResult .Count;idx++) 
            {
                var x = tableResult[idx].Result as Customers;
                Console.WriteLine($"Customer Info : User Name : {x.Name} , Age : {x.Age} , Gender : {x.Gender}");
            }
        }

        public async static Task GetEntity()
        {
            var tableRef = InitializeTableClient();

            var operation = TableOperation.Retrieve<Customers>(partitionKey, rowKey);
            var tableResult = await tableRef.ExecuteAsync(operation);
            var result = tableResult.Result as Customers;

            Console.WriteLine($"Customer Info : User Name : {result.Name} , Age : {result.Age} , Country :");
        }


        public async static Task UpdateEntity()
        {
            var tableRef = InitializeTableClient();

            var customer = new Customers("CA-01", 33, "Male", "Egypt", "CA-01");
            var operation = TableOperation.InsertOrMerge(customer);
            var tableResult = await tableRef.ExecuteAsync(operation);
            var result = tableResult.Result as Customers;
            Console.WriteLine($"Customer Info : User Name : {result.Name} , Age : {result.Age} , Country :");
        }

        public async static Task DeleteEntity()
        {
            var tableRef = InitializeTableClient();

            var operation = TableOperation.Retrieve<Customers>(partitionKey, rowKey);
            var getTableOperation = await tableRef.ExecuteAsync(operation);
            var custResult = getTableOperation.Result as Customers;

            var operation2 = TableOperation.Delete(custResult);
            var tableResult = await tableRef.ExecuteAsync(operation2);
            var result = tableResult.Result as Customers;
            Console.WriteLine($"Customer Info : User Name : {result.Name} , Age : {result.Age} , Country :");
        }
    }
}
