using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DoctorClientModule.DB;
using DoctorClientModule.Navigation;
using DoctorClientModule.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace DoctorClientModule.ViewModel
{
    public class NewPatientViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private IFrameNavigationService _navigationService;

        private Pacient Patients { get; set; }
        private Doctor Doctor { get; set; }

        public NewPatientViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            Messenger.Default.Register<Doctor>(this, "Doctor", doc =>
            {
                Doctor = doc;

            });

            AddPacient = new RelayCommand<Pacient>(async pat =>
            {
                Pacient pt = await AddPatientUtilities.AddPatient(pat, Doctor);
                if (pt != null)
                {
                    Messenger.Default.Send<Pacient>(pt, "Patient");
                    _navigationService.NavigateTo("Doctor");
                }
            });
        }

        public RelayCommand<Pacient> AddPacient { get; set; }
    }
}
