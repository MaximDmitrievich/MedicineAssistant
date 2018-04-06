using System;
using System.Net;
using System.Net.Http;
using FunctionHandling.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace FunctionHandling
{
    public static class Handling
    {
        [FunctionName("Handling")]
        public static async System.Threading.Tasks.Task<HttpResponseMessage> RunAsync(HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            string resultData = "";
            string data = await req.Content.ReadAsStringAsync();
            HttpResponseMessage result;

            if (data.Length > 2000)
            {
                result = new HttpResponseMessage(HttpStatusCode.RequestEntityTooLarge);
            }
            else
            {
                MessageData mdata = JsonConvert.DeserializeObject<MessageData>(data);
                DeviceData devicedata = new DeviceData
                {
                    Temperature = mdata.Temperature,
                    Cardio = mdata.Cardio,
                    EventProcessedUtcTime = mdata.EventProcessedUtcTime,
                    HR = mdata.HR
                };

                HumanData newdata = new HumanData
                {
                    DeviceId = mdata.DeviceId,
                    Data = devicedata
                };

                resultData = JsonConvert.SerializeObject(newdata);
                HttpContent content = new StringContent(resultData);

                using (HttpClient client = new HttpClient())
                {
                    var requestUri = new Uri("http://maconcentrator.westeurope.cloudapp.azure.com:5200/api/datasaver");
                    var request = new HttpRequestMessage
                    {
                        RequestUri = requestUri,
                        Method = HttpMethod.Post,
                        Content = content
                    };
                    request.Headers.Add("Accept", "application/json");
                    var response = await client.SendAsync(request);
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        result = new HttpResponseMessage(HttpStatusCode.OK);
                    }
                    else
                    {
                        result = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
            }

            return req.CreateResponse(result.StatusCode, resultData);
        }
    }
}
