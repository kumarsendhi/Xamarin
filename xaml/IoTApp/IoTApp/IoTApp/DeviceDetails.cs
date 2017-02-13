using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace IoTApp
{
    public class DeviceDetails
    {
        [JsonProperty (PropertyName ="id")]
        public string Id { get; set; }

        [JsonProperty (PropertyName ="temperature")]
        public string Temperature { get; set; }

        [JsonProperty (PropertyName = "Humidity")]
        public string Humidity { get; set; }

        [JsonProperty (PropertyName = "action")]
        public bool Action { get; set; }
    }
}
