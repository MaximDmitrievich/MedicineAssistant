using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionHandling.Models
{
    public class DeviceData
    {
        public double Temperature { get; set; }
        public double HR { get; set; }
        public List<double> Cardio { get; set; }
        public DateTime EventProcessedUtcTime { get; set; }
    }
}
