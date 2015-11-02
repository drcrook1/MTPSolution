using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTP.DeviceCore.TelemetryObj
{
    public class TemperatureEvent
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public float Temperature { get; set; }
        public DateTime CollectionTime { get; set; }
        public string DeviceId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
