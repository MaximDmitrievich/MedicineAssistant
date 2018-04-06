using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebSendingASP.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace WebSendingASP.Controllers
{
    [Route("api/humandata")]
    public class HumanDataController : Controller
    {
        public HumanDataController(HumanSending hdatas)
        {
            HumanDatas = hdatas;
        }

        public HumanSending HumanDatas { get; set; }

        public IEnumerable<HumanData> GetAll()
        {
            return HumanDatas.DataList;
        }

        [HttpPost]
        public IActionResult Create([FromBody] HumanData hdata)
        {
            if (hdata == null)
            {
                return BadRequest();
            }
            int needed = 0;
            bool isExist = false;
            for (int i = 0; i < HumanDatas.DataList.Count; i++)
            {
                if (HumanDatas.DataList[i].DeviceId == hdata.DeviceId)
                {
                    needed = i;
                    isExist = true;
                    break;
                }
            }
            if (!isExist)
            {
                HumanDatas.DataList.Add(hdata);
            }
            else
            {
                HumanDatas.DataList[needed].Data = HumanDatas.DataList[needed].Data.Concat(hdata.Data).ToList();
            }
            return CreatedAtRoute("GetData", new {deviceid = hdata.DeviceId}, hdata);
        }
    }
}
