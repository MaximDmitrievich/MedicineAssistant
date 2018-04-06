using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DoctorClientModule.DB;
using Newtonsoft.Json;

namespace DoctorClientModule.Utilities
{
    public static class DoctorUtilities
    {
        public static async Task<ObservableCollection<Pacient>> GetPatients(Doctor doctor)
        {
            ObservableCollection<Pacient> result = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMilliseconds(1000);
                    using (HttpRequestMessage request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(Consts.Dependency + $"/doctorDep/{doctor.Id}"),
                        Method = HttpMethod.Get,
                    })
                    {
                        request.Headers.Add("Accept", "application/json");

                        HttpResponseMessage response;

                        response = await client.SendAsync(request);
                        

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            string tmp = await response.Content.ReadAsStringAsync();
                            var tmpobj = JsonConvert.DeserializeObject<ObservableCollection<Pacient>>(tmp);
                            if (!Equals(tmpobj, null))
                            {
                                result = tmpobj;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return result;
        }
    }
}
