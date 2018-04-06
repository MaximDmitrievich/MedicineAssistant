using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using TodoApi.Models.DeviceManager;

namespace TodoApi.Models
{
    public static class StorageConnecting
    {
        private static readonly TimeSpan TwentySecSpan = TimeSpan.FromSeconds(20);

        private static CloudStorageAccount StorageAccount { get; } =
            CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;" +
                                      "AccountName=medicineassistant;" +
                                      "AccountKey=7RTxe5nrZb09eI+P1wvrZcAzcOTY4qWhzWGmvMsS" +
                                      "ntWw1YAXpEu0k4MoJmdpoBfUlCoFsX5y/Au5D94n63zUMg==;" +
                                      "EndpointSuffix=core.windows.net");

        private static CloudBlobClient BlobClient { get; } =
            StorageAccount.CreateCloudBlobClient();

        private static CloudBlobContainer BlobContainer { get; } =
            BlobClient.GetContainerReference("testcontainer");
        //private static CloudBlockBlob BlockBlobBlob { get; set; }

        public static async Task SendFile(string json, string deviceid, DateTime date)
        {
            var name = $@"{date.Year}_{date.Month}_{date.Day}_{date.Hour}_{date.Minute}_{date.Second}.json";
            await BlobContainer.CreateIfNotExistsAsync();
            var blockBlobBlob = BlobContainer.GetBlockBlobReference($@"{deviceid}/" + name);
            blockBlobBlob.Properties.ContentType = "application/json";
            await blockBlobBlob.UploadTextAsync(json);
        }

        public static async Task<IEnumerable<Tuple<DateTime, DateTime>>> GetChartInfo(string deviceid)
        {
            List<Tuple<DateTime, DateTime>> resChartInf = new List<Tuple<DateTime, DateTime>>();
            BlobContinuationToken continuationToken = null;
            List<CloudBlockBlob> result = new List<CloudBlockBlob>();

            do
            {
                var resultSegment = await BlobContainer.ListBlobsSegmentedAsync(deviceid, true, BlobListingDetails.All,
                    20, continuationToken, null, null);
                result.AddRange(resultSegment.Results.Cast<CloudBlockBlob>());
                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);

            //= new Tuple<DateTime, DateTime>(DateTime.Now, DateTime.Now);

            if (result.Count > 0)
            {
                result.Sort((x, y) =>
                {
                    Tuple<DateTime, DateTime> first = ParseFileName(x.Name);
                    Tuple<DateTime, DateTime> sec = ParseFileName(x.Name);
                    return DateTime.Compare(first.Item1, sec.Item1);
                });
                List<Tuple<DateTime, DateTime>> resultTmpList = new List<Tuple<DateTime, DateTime>>();
                foreach (var item in result)
                {
                    resultTmpList.Add(ParseFileName(item.Name));
                }

                Tuple<DateTime, DateTime> last = ParseFileName(result[0].Name);
                var lastStart = last.Item1;
                var lastEnd = last.Item2;
                foreach (var item in result)
                {
                    Tuple<DateTime, DateTime> tmp = ParseFileName(item.Name);
                    if (tmp.Item2 - lastEnd < TwentySecSpan)
                    {
                        lastEnd = tmp.Item2;
                    }
                    else if (tmp.Item2 - lastEnd >= TwentySecSpan)
                    {
                        resChartInf.Add(new Tuple<DateTime, DateTime>(lastEnd, lastStart));
                        lastStart = tmp.Item1;
                        lastEnd = tmp.Item2;
                    }
                }

                resChartInf.Sort((x, y) => DateTime.Compare(x.Item1, y.Item1));
            }

            return resChartInf;
        }

        public static async Task<HumanData> GetFiles(string deviceid, DateTime begin, DateTime end)
        { 
            //begin = begin.ToUniversalTime();
            //end = end.ToUniversalTime();
            BlobContinuationToken continuationToken = null;
            List<CloudBlockBlob> result = new List<CloudBlockBlob>();
            do
            {
                var resultSegment = await BlobContainer.ListBlobsSegmentedAsync(deviceid, true, BlobListingDetails.All,
                    20, continuationToken, null, null);
                result.AddRange(resultSegment.Results.Cast<CloudBlockBlob>());
                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);

            result = result.Where(x =>
            {
                Tuple<DateTime, DateTime> tmp = ParseFileName(x.Name);
                return tmp.Item1 >= begin && tmp.Item2 <= end;
            }).ToList();

            var resultData = new HumanData
            {
                DeviceId = deviceid,
                Data = new List<DeviceData>()
            };

            foreach (var item in result)
            {
                resultData.Data.AddRange(
                    JsonConvert.DeserializeObject<HumanData>(
                        await item.DownloadTextAsync()).Data);
            }

            resultData.Data.Sort((x, y) =>
                DateTime.Compare(x.EventProcessedUtcTime, y.EventProcessedUtcTime));
            GC.Collect();
            //TODO HumanDataOptimization(ref resultData, begin, end);
            return resultData;
        }

        /* 
        #region Constans
        private static long SecondTicks = new DateTime(0, 0, 0, 1, 0, 0).ToUniversalTime().Ticks;
        private static long HourTicks = new DateTime(0, 0, 0, 1, 0, 0).ToUniversalTime().Ticks;
        private static long MinuteTicks = new DateTime(0, 0, 0, 0, 1, 0).ToUniversalTime().Ticks;
        private static long DayTicks = new DateTime(0, 0, 1).ToUniversalTime().Ticks;
        private static long WeekTicks = new DateTime(0, 0, 7).ToUniversalTime().Ticks;
        private static long MonthTicks = new DateTime(0, 1, 0).ToUniversalTime().Ticks;
        #endregion

        private static void HumanDataOptimization(ref HumanData hdata, DateTime begin, DateTime end)
        {
            long Diff(DateTime beginT, DateTime endT)
            {
                return endT.ToUniversalTime().Ticks - beginT.ToUniversalTime().Ticks;
            }

            IEnumerable<DeviceData> tmp;
            long diff = Diff(begin, end);

            if (diff < HourTicks && diff > HourTicks / 2)
            {
                while (Diff(begin, end) > 0)
                {
                    begin.AddTicks(5 * SecondTicks);
                    tmp = hdata.Data.TakeWhile(x=>Diff(x.EventProcessedUtcTime, begin) >= 0);
                }
            }
        }
        */
        //MUSOR8/2018|2|4|20|59|13|119_2018|2|4|20|59|13|119.json
        private static Tuple<DateTime, DateTime> ParseFileName(string str)
        {
            Tuple<DateTime, DateTime> result = new Tuple<DateTime, DateTime>(new DateTime(), new DateTime());

            int ToInt(ref string refStr)
            {
                int.TryParse(refStr, out var res);
                return res;
            }

            DateTime GetDateTime(string inp)
            {
                var resultDateTime = new DateTime();
                if (!string.IsNullOrEmpty(inp) && inp.Any())
                {
                    string[] inpMass = inp.Split('|');
                    resultDateTime = new DateTime(ToInt(ref inpMass[0]),
                        ToInt(ref inpMass[1]),
                        ToInt(ref inpMass[2]),
                        ToInt(ref inpMass[3]),
                        ToInt(ref inpMass[4]),
                        ToInt(ref inpMass[5]),
                        ToInt(ref inpMass[6]));
                }

                return resultDateTime;
            }

            if (!string.IsNullOrEmpty(str) && str.Any())
            {
                var name = str.Split('/', '.')[1];
                string[] parts = name.Split('_');
                result = new Tuple<DateTime, DateTime>(GetDateTime(parts[0]), GetDateTime(parts[1]));
            }

            return result;
        }
    }
}