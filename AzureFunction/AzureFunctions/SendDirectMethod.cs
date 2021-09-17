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
using AzureFunctions.Models;

namespace AzureFunctions
{ 
    public static class SendDirectMethod
    {
        private static readonly ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("IoTHubConnection"));

        [FunctionName("SendDirectMethod")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "directmethod")] HttpRequest req,
            ILogger log)
        {            
            var data = JsonConvert.DeserializeObject<DirectMethodModel>(await new StreamReader(req.Body).ReadToEndAsync());
            var method = new CloudToDeviceMethod(data.MethodName);            

            CloudToDeviceMethodResult result = await serviceClient.InvokeDeviceMethodAsync(data.DeviceId, method);
            
            if (result.Status != 200)
                return new BadRequestResult();
            else
                return new OkResult();                 
        }
    }
}
