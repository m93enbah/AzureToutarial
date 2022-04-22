using Microsoft.Azure.Cosmos.Table;

namespace AzureStorageTable
{
    public class Customers : TableEntity
    {
        public string Name { get; set; }
        public long Age { get; set; }
        public string Gender { get; set; }

        public Customers()
        {
        }

        public Customers(string name, long age, string gender, string country, string custID) 
        {
            RowKey = custID;
            PartitionKey = country;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }
}
