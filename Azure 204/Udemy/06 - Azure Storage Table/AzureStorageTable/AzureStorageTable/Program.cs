using Microsoft.Azure.Cosmos.Table;
using System;
using System.Threading.Tasks;

namespace AzureStorageTable
{
    public class Program
    {
        static async Task Main(string[] args)
        {
     //      await  AzureTableStorage.InitializeStorageAccount();
     //       await AzureTableStorage.AddEntity();
    //        await AzureTableStorage.AddEntities();
            await AzureTableStorage.GetEntity();
            await AzureTableStorage.UpdateEntity();
            await AzureTableStorage.DeleteEntity();
            Console.ReadLine();
        }
    }
}
