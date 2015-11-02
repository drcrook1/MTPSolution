using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTP.DeviceCore.TelemetryObj
{
    public class LightEvent
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public float LightRatio { get; set; }
        public DateTime CollectionTime { get; set; }
    }
}
