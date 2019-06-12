namespace IcalFilter
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Net;
    using Ical.Net.Serialization;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using IcalFilter.Filters;
    using IcalFilter.Entities;

    public static class AzureFunctionIcalFilter
    {
        [FunctionName("IcalFilter")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string calendarId = req.Query["id"];
            calendarId = calendarId ?? data?.id;

            var cloudTable = await CreateCalendarsTableAsync();
            var calendar = await GetCalendar(cloudTable, calendarId);
            if(calendar == null)
            {
                throw new Exception("Calendar not found");
            }

            Ical.Net.Calendar c;
            Ical.Net.Calendar outc = new Ical.Net.Calendar();
            using (var response = await WebRequest
                .Create(calendar.Url)
                .GetResponseAsync())

            using (var stream = response.GetResponseStream())
            {
                c = Ical.Net.Calendar.Load(stream);
            }

            var filter = CalendarFilterFactory.GetFilter(calendar.Filter);

            foreach (var e in c.Events)
            {
                if(filter.IsMatch(e))
                {
                    outc.Events.Add(e);
                }
            }

            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(outc);

            return (ActionResult)new OkObjectResult(serializedCalendar);
        }

        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        }

        public static CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            return storageAccount;
        }

        public static async Task<CloudTable> CreateCalendarsTableAsync()
        {
            string tableName = "Calendars";
            var storageConnectionString = GetConnectionString();
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(storageConnectionString);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public static async Task<Calendar> GetCalendar(CloudTable table, string rowKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<Calendar>("All", rowKey);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            Calendar customer = result.Result as Calendar;
            return customer;
        }
    }
}
