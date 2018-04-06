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
    public static class AddPatientUtilities
    {
        public static async Task<Pacient> AddPatient(Pacient pat, Doctor doctor)
        {
            try
            {
                using(HttpClient client = new HttpClient())
                {
                    using (HttpRequestMessage request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(Consts.Pacients +
                                          $"/{pat.SNILS}/{pat.BirthdayDate.Year}-{pat.BirthdayDate.Month}-{pat.BirthdayDate.Day}"),
                        Method = HttpMethod.Get
                    })
                    {
                        request.Headers.Add("Accept", "application/json");
                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                string isExist = await response.Content.ReadAsStringAsync();
                                if (isExist == "false")
                                {
                                    using (HttpRequestMessage requestAddPatient = new HttpRequestMessage
                                    {
                                        RequestUri = new Uri(Consts.Pacients),
                                        Method = HttpMethod.Post
                                    })
                                    {
                                        requestAddPatient.Headers.Add("Accept", "application/json");
                                        requestAddPatient.Content = new StringContent(JsonConvert.SerializeObject(pat));
                                        using (
                                            HttpResponseMessage responseAddPatient =
                                                await client.SendAsync(requestAddPatient))
                                        {
                                            if (responseAddPatient.StatusCode == HttpStatusCode.OK)
                                            {
                                                string added = await responseAddPatient.Content.ReadAsStringAsync();
                                                if (added == "Pacient have been added")
                                                {
                                                    using (HttpRequestMessage requestPatient = new HttpRequestMessage
                                                    {
                                                        RequestUri = new Uri(Consts.Pacients +
                                                    $"/{pat.SNILS}/{pat.BirthdayDate.Year}-{pat.BirthdayDate.Month}-{pat.BirthdayDate.Day}"),
                                                        Method = HttpMethod.Post
                                                    })
                                                    {
                                                        requestPatient.Headers.Add("Accept", "application/json");
                                                        requestPatient.Content = new StringContent("");
                                                        using (
                                                            HttpResponseMessage responsePatient =
                                                                await client.SendAsync(requestPatient))
                                                        {
                                                            if (response.StatusCode == HttpStatusCode.OK)
                                                            {
                                                                using (
                                                                    HttpRequestMessage requestDependencies = new HttpRequestMessage
                                                                    {
                                                                        RequestUri = new Uri(Consts.Dependency + $"/{pat.Id}/{doctor.Id}"),
                                                                        Method = HttpMethod.Post
                                                                    })
                                                                {
                                                                    using (
                                                                        HttpResponseMessage responseDependeincies =
                                                                            await client.SendAsync(requestDependencies))
                                                                    {
                                                                        if (responseDependeincies.StatusCode ==
                                                                            HttpStatusCode.OK)
                                                                        {
                                                                            MessageBox.Show(
                                                                                $"Patient {pat.Name} {pat.LastName} {pat.Patronymic} have been added",
                                                                                "Add Patient", MessageBoxButton.OK,
                                                                                MessageBoxImage.Information);
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
                                }
                            }
                            else
                            {
                                MessageBox.Show($"This patient is already added to database", "Add Patient", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Add Patient", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return pat;
        }
    }
}
