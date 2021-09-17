using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AzureFunctions
{
    public static class GetItems
    {
        [FunctionName("GetItems")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "IoT", collectionName: "Data", CreateIfNotExists = true, ConnectionStringSetting = "CosmosDB", SqlQuery ="SELECT * FROM c" )] IEnumerable<dynamic> cosmos,
            ILogger log)
        {
            if (cosmos == null)
            {
                log.LogInformation($"Requested items not found.");
                return new NotFoundResult();
            }
            log.LogInformation($"Requested items found");
            return new OkObjectResult(cosmos);
        }
    }
}
