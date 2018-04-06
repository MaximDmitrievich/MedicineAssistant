using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.Threading;

namespace WebSendingASP.Models
{
    public class HumanSending
    {
        private List<HumanData> _datalist;
        private StorageConnecting _sc;
        private Timer _tm = null;
        
        AutoResetEvent _autoEvent = null;

        private int invokeCount = 0;
        private int maxCount = 5;

        public List<HumanData> DataList
        {
            get { return _datalist; }
            set { _datalist = value; }
        }

        public HumanSending()
        {
            _datalist = new List<HumanData>();
            _sc = new StorageConnecting();
            _autoEvent = new AutoResetEvent(false);
            _tm = new Timer(Send, _autoEvent, 40000, 40000);
        }

        public void Add(HumanData item)
        {
            _datalist.Add(item);
        }

        public async void Send(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            List<HumanData> copy = new List<HumanData>(_datalist);
            _datalist.Clear();
            if (copy.Count > 0)
            {
                for (int i = 0; i < copy.Count; i++)
                {
                    string json = JsonConvert.SerializeObject(copy[i]);
                    await _sc.SendFile(json, copy[i].DeviceId, copy[i].Data[0].EventProcessedUtcTime);
                }
            }

            if (invokeCount == maxCount)
            {
                // Reset the counter and signal the waiting thread.
                invokeCount = 0;
                autoEvent.Set();
            }
        }
        ~HumanSending()
        {
            _tm.Dispose();
            _autoEvent.Dispose();
            DataList.Clear();
        }
    }

}
