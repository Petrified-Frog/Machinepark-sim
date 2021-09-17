using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctions.Models;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctions
{
    public static class GetItem
    {
        [FunctionName("GetItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "devices/{id}")] HttpRequest req,
            [CosmosDB(
            databaseName: "IoT", 
            collectionName: "Data", 
            CreateIfNotExists = true, 
            ConnectionStringSetting = "CosmosDB",
            SqlQuery = "select top 1 * from c where c.DeviceId = {id} order by c._ts desc"
            )] IEnumerable<IotDevice> devices,
            ILogger log)
        {
            if (devices == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(devices.FirstOrDefault());
        }
    }
}
