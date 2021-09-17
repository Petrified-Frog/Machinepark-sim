using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctions.Models
{
    public class IotDevice
    {
        public string DeviceId { get; set; } = "Not available";
        public string DeviceName { get; set; } = "Not available";
        public string ConnectionState { get; set; } = "Offline";
        public string Status { get; set; } = "Disabled";
        public string JsonData { get; set; } = "No data present";
        public DateTime JsonDataLastUpdated { get; set; } = DateTime.Now;
        public bool Sending { get; set; } = false;
    }
}
