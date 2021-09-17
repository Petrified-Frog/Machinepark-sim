using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace PrinterSimulator
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

            bool jammed = false;
            string status = "Ok";
            var rand = new Random();
            while (true)
            {
                while (sending)
                {
                    if (rand.Next(100) < 11)
                    {
                        status = "Jammed";
                        jammed = true;
                    }

                    var data = JsonConvert.SerializeObject(new { status = status });
                    Console.Write("Message sent: " + data + "\n");
                    var message = new Message(Encoding.UTF8.GetBytes(data));
                    message.Properties["devicename"] = "6502c8ed-bd22-45ca-b3cc-37d5e64cf574";
                    await deviceClient.SendEventAsync(message);

                    await Task.Delay(10 * 1000);
                }
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
