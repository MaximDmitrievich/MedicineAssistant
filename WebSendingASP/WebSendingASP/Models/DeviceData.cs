using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSendingASP.Models
{
    public class DeviceData
    {
        public double Temperature { get; set; }
        public double HR { get; set; }
        public List<double> Cardio { get; set; }
        public DateTime EventProcessedUtcTime { get; set; }
    }
}
