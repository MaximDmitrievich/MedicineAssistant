using System;
using System.Collections.Generic;
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
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;

namespace DoctorClientModule.ViewModel
{
    public class SignViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private IFrameNavigationService _navigationService;

        public Doctor Doctor { get; set; }

        public SignViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            SignIn = new RelayCommand<Tuple<string, string>>(async logpas =>
            {
                Doctor = await SignUtilities.GetDoctor(logpas);
                RaisePropertyChanged(nameof(Doctor));
                Messenger.Default.Send<Doctor>(Doctor,"Doctor");
                _navigationService.NavigateTo("Doctor");
            });
            SignUp = new RelayCommand<Doctor>(async doc =>
            {
                Doctor = await SignUtilities.NewDoctor(doc);
                RaisePropertyChanged(nameof(Doctor));
                _navigationService.NavigateTo("Doctor");
            });
        }
        
        public RelayCommand<Tuple<string, string>> SignIn { get; set; }
        public RelayCommand<Doctor> SignUp { get; set; }
    }
}
