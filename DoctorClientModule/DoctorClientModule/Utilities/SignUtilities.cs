using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DoctorClientModule.DB;
using Newtonsoft.Json;

namespace DoctorClientModule.Utilities
{
    public static class SignUtilities
    {
        public static async Task<Doctor> NewDoctor(Doctor doc)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(Consts.Doctors),
                        Method = HttpMethod.Post
                    })
                    {
                        request.Headers.Add("Accept", "application/json");
                        request.Content = new StringContent(JsonConvert.SerializeObject(doc));
                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                string tmp = await response.Content.ReadAsStringAsync();
                                var tmpobj = JsonConvert.DeserializeObject<Doctor>(tmp);
                                if (!Equals(tmpobj, null))
                                {
                                    doc = tmpobj;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "New Doctor", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return doc;
        }

        public static async Task<Doctor> GetDoctor(Tuple<string, string> lp)
        {
            Doctor result = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(Consts.Doctors + $"/{lp.Item1}/{lp.Item2}"),
                        Method = HttpMethod.Get
                    })
                    {
                        request.Headers.Add("Accept", "application/json");
                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                string isExist = await response.Content.ReadAsStringAsync();
                                if (isExist == "true")
                                {
                                    using (HttpRequestMessage requestDoctor = new HttpRequestMessage
                                    {
                                        RequestUri = new Uri(Consts.Doctors + $"/{lp.Item1}/{lp.Item2}"),
                                        Method = HttpMethod.Post
                                    })
                                    {
                                        requestDoctor.Headers.Add("Accept", "application/json");
                                        requestDoctor.Content = new StringContent("");
                                        using (
                                            HttpResponseMessage responseDoctor = await client.SendAsync(requestDoctor))
                                        {
                                            if (responseDoctor.StatusCode == HttpStatusCode.OK)
                                            {
                                                string tmp = await responseDoctor.Content.ReadAsStringAsync();
                                                var tmpobj = JsonConvert.DeserializeObject<Doctor>(tmp);
                                                if (!Equals(tmpobj, null))
                                                {
                                                    result = tmpobj;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Get Doctor", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }
    }
}
