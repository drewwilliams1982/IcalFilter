namespace IcalFilter.Entities
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class Calendar : TableEntity
    {
        public Calendar()
        {
            this.PartitionKey = "All";
            this.RowKey = System.Guid.NewGuid().ToString("N");
        }

        public string Url { get; set; }

        public string Filter { get; set; }
    }
}