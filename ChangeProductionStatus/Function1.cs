using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using System.Threading.Tasks;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;

namespace ChangeProductionStatus
{
    public class Function1
    {
        /*private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
        }

        [Function("Function1")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }*/
        private static readonly RegistryManager registryManager =
       RegistryManager.CreateFromConnectionString("HostName=zajeciaiot.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=ZS1qhD72UC7wWqYp+jyo1+ZA/oFTDCCUeAIoTI7w/Ig=");

        [FunctionName("SetProductionStatus")]
        public static async Task<IActionResult> Run(
            [ Microsoft.Azure.WebJobs.HttpTrigger(
            Microsoft.Azure.WebJobs.Extensions.Http.AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string deviceId = data?.deviceId;
            int newStatus = data?.status;

            if (string.IsNullOrEmpty(deviceId) || data?.status == null)
            {
                return new BadRequestObjectResult("Missing deviceId or status");
            }

            var twin = await registryManager.GetTwinAsync(deviceId);
            twin.Properties.Desired["ProductionStatus"] = newStatus;

            await registryManager.UpdateTwinAsync(deviceId, twin, twin.ETag);
            return new OkObjectResult($"Set ProductionStatus to {newStatus} for device {deviceId}");
        }
    }
}
