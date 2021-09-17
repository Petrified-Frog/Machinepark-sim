using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace OvenSimulation
{
    class Program
    {        
        public static readonly DeviceClient deviceClient = DeviceClient.CreateFromConnectionString("IoT Device connectionstring", TransportType.Mqtt);
        public static bool sending = true;
        static async Task Main(string[] args)
        {
            await deviceClient.SetMethodHandlerAsync("start", Start, null);
            await deviceClient.SetMethodHandlerAsync("stop", Stop, null);

            await UpdateReportedSendingAsync();


            int temp = 60;
            string status = "Ok";
            var rand = new Random();
            while (true)
            {
                while (sending)
                {
                    if (rand.Next(0, 2) == 0)
                        temp++;
                    else
                        temp--;

                    if (temp > 80)
                        status = "Overheated";
                    else
                        status = "Ok";

                    var data = JsonConvert.SerializeObject(new { temperature = temp, status = status });
                    var message = new Message(Encoding.UTF8.GetBytes(data));
                    message.Properties["devicename"] = "ae1cf220-92ba-4d31-957e-dd5ef2ba9617";
                    await deviceClient.SendEventAsync(message);
                    Console.Write("Message sent: " + data + "\n");

                    await Task.Delay(10 * 1000);
                }
                await Task.Delay(500);
            }
        }

        //Adds/Updates a properties variable in device twin
        private static async Task UpdateReportedSendingAsync()
        {
            var twin = await deviceClient.GetTwinAsync();
            string patch = JsonConvert.SerializeObject(new { sending }); //sending is short for sending = sending
            await deviceClient.UpdateReportedPropertiesAsync(JsonConvert.DeserializeObject<TwinCollection>(patch));
        }

        //Called via direct method azurefunction from webpage
        private static async Task<MethodResponse> Start(MethodRequest methodRequest, Object userContext)
        {
            sending = true;
            Console.Write("Messages started\n");
            await UpdateReportedSendingAsync();
            return new MethodResponse(new byte[0], 200);
        }
        //Called via direct method azurefunction from webpage
        private static async Task<MethodResponse> Stop(MethodRequest methodRequest, Object userContext)
        {
            sending = false;
            Console.Write("Messages stopped\n");
            await UpdateReportedSendingAsync();
            return new MethodResponse(new byte[0], 200);
        }
    }
}
