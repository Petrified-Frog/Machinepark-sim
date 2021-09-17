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
using System.Linq;

namespace AzureFunctions
{
    public static class GetDevicesInformation
    {
        private static readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IoTHubConnection"));
        [FunctionName("GetDevicesInformation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var result = registryManager.CreateQuery($"SELECT * FROM devices");
            var data = await result.GetNextAsTwinAsync();

            if (data.Any())
            {
                var json = JsonConvert.SerializeObject(data);
                return new OkObjectResult(json);
            }
            else
                return new NotFoundResult();
        }
    }
}
