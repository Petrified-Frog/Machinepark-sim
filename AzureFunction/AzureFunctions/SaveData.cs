using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Management.EventHub.Models;
using AzureFunctions.Models;

namespace AzureFunctions
{
    public static class SaveData
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("SaveData")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "IotHubEndpoint", ConsumerGroup = "cosmos")] EventData message,
            [CosmosDB(databaseName: "IoT", collectionName: "Data", CreateIfNotExists = true, ConnectionStringSetting = "CosmosDB")] out dynamic cosmos,
            ILogger log)
        {
            try
            {
                cosmos = new IotDevice
                {
                    DeviceId = message.SystemProperties["iothub-connection-device-id"].ToString(),
                    DeviceName = message.Properties["devicename"].ToString(),
                    JsonData = Encoding.UTF8.GetString(message.Body.Array),

                };
            }
            catch
            {
                cosmos = null;
            }
        }
    }
}