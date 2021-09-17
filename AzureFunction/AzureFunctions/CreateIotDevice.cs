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

namespace AzureFunctions
{
    public static class CreateIotDevice
    {
        private static readonly RegistryManager registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IoTHubConnection"));
        [FunctionName("CreateIotDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(requestBody);

            if (data != null)
            {
                await registryManager.AddDeviceAsync(new Device(data.deviceId));
                return new OkResult();
            }
            else
                return new BadRequestResult();
        }
    }
}
