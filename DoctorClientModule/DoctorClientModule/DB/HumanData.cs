using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorClientModule.DB
{
    public class HumanData
    {
        public string DeviceId { get; set; }
        public List<DeviceData> Data { get; set; }
    }
}
