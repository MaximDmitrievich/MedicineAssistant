using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorClientModule.DB
{
    public class DeviceData
    {
        public List<double> Temperature { get; set; }
        public List<double> HR { get; set; }
        public List<double> Cardio { get; set; }
        public List<long> Ticks { get; set; }
    }
}
