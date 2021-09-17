using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Collections.Generic;
using AzureFunctions.Models;
using System.Net.Http;

namespace AzureFunctions
{
    public static class GetTwinDevices
    {
        private static readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IoTHubConnection"));
        private static HttpClient client = new HttpClient();

        [FunctionName("GetTwinDevices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var devices = new List<IotDevice>();

            var result = registryManager.CreateQuery($"SELECT * FROM devices");
            var twins = await result.GetNextAsTwinAsync();

            foreach(var twin in twins)
            {
                var response = await client.GetAsync(Environment.GetEnvironmentVariable("GetDeviceByIdUrl")+twin.DeviceId);
                var data = JsonConvert.DeserializeObject<IotDevice>(await response.Content.ReadAsStringAsync());

                devices.Add(new IotDevice
                {
                    DeviceId = twin.DeviceId,
                    DeviceName = data.DeviceName,
                    ConnectionState = (twin.ConnectionState.ToString() == "Connected") ? "Online" : "Offline",
                    Status = twin.Status.ToString(),
                    JsonData = data.JsonData,
                    JsonDataLastUpdated = data.JsonDataLastUpdated,
                    Sending = twin.Properties.Reported["sending"]
                });
            }

            return new OkObjectResult(devices);
        }
    }
}
