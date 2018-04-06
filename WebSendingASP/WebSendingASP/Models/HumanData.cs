using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSendingASP.Models
{
    public class HumanData
    {
        public string DeviceId { get; set; }
        public List<DeviceData> Data { get; set; }
    }
}
