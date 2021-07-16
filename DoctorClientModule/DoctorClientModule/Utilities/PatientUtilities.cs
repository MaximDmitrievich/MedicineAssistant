using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DoctorClientModule.DB;
using DoctorClientModule.Views;
using InteractiveDataDisplay.WPF;
using Newtonsoft.Json;

namespace DoctorClientModule.Utilities
{
    public static class PatientUtilities
    {
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static async Task<List<Tuple<DateTime, DateTime>>> GetRanges(Pacient pat)
        {
            List<Tuple<DateTime, DateTime>> daterange = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(Consts.Chart + $@"/{pat.DeviceID}"),
                        Method = HttpMethod.Get
                    })
                    {
                        request.Headers.Add("Accept", "application/json");

                        HttpResponseMessage response = await client.SendAsync(request);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            string tmp = await response.Content.ReadAsStringAsync();
                            var tmpobj = JsonConvert.DeserializeObject<List<Tuple<DateTime, DateTime>>>(tmp);
                            if (!Equals(tmpobj, null))
                            {
                                daterange = tmpobj;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Load Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return daterange;
        }

        public static async Task<List<EmotionData>> GetEmotion(Pacient pat)
        {
            List<EmotionData> result = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(Consts.Emo + $@"/{pat.Id}"),
                        Method = HttpMethod.Get
                    })
                    {
                        request.Headers.Add("Accept", "application/json");

                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                string tmp = await response.Content.ReadAsStringAsync();
                                var tmpobj = JsonConvert.DeserializeObject<List<EmotionData>>(tmp);
                                if (!Equals(tmpobj, null))
                                {
                                    result = tmpobj;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Get Emotion", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }

        public static async Task<string> GetDisease(Pacient pat)
        {
            string result = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(Consts.Dis + $@"/{pat.Id}"),
                        Method = HttpMethod.Get
                    })
                    {
                        request.Headers.Add("Accept", "application/json");

                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                string tmp = await response.Content.ReadAsStringAsync();
                                var tmpobj = JsonConvert.DeserializeObject<string>(tmp);
                                if (!Equals(tmpobj, null))
                                {
                                    result = tmpobj;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Get Emotion", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return result;
        }

        public static async Task<Tuple<DateTime, DateTime>> GetLastRange(Pacient pat)
        {
            Tuple<DateTime, DateTime> range = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(Consts.Chart + $@"/first/{pat.DeviceID}"),
                        Method = HttpMethod.Get
                    })
                    {
                        request.Headers.Add("Accept", "application/json");

                        HttpResponseMessage response = await client.SendAsync(request);

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            string tmp = await response.Content.ReadAsStringAsync();
                            var tmpobj = JsonConvert.DeserializeObject<Tuple<DateTime, DateTime>>(tmp);
                            if (!Equals(tmpobj, null))
                            {
                                range = tmpobj;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Last Range", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return range;
        }

        public static async Task<HumanData> LoadHumanData(Pacient pat, Tuple<DateTime, DateTime> daterange)
        {
            HumanData data = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    StringBuilder builder = new StringBuilder(Consts.Chart);
                    builder.Append("/");
                    builder.Append(pat.DeviceID);
                    builder.Append("/");
                    string b = JsonConvert.SerializeObject(daterange.Item1);
                    builder.Append(b.Substring(1, b.Length - 2));
                    builder.Append("/");
                    b = JsonConvert.SerializeObject(daterange.Item2);
                    builder.Append(b.Substring(1, b.Length - 2));

                    using (HttpRequestMessage request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(builder.ToString()),
                        Method = HttpMethod.Get
                    })
                    {
                        request.Headers.Add("Accept", "application/json");

                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                string tmp = await response.Content.ReadAsStringAsync();
                                var tmpobj = JsonConvert.DeserializeObject<HumanData>(tmp);
                                if (!Equals(tmpobj, null))
                                {
                                    data = tmpobj;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Load Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return data;
        }
    }
}
