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
using System.Windows.Input;
using DoctorClientModule.DB;
using DoctorClientModule.Navigation;
using DoctorClientModule.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace DoctorClientModule.ViewModel
{
    public class DoctorViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private IFrameNavigationService _navigationService;

        public Doctor Doctor { get; set; }
        public ObservableCollection<Pacient> Patients { get; set; }

        private Pacient _secItem;
        
        public Pacient SecItem {
            get { return _secItem; }
            set
            {
                _secItem = value;
                Messenger.Default.Send<Pacient>(_secItem, "Patient");
                _secItem = null;
                _navigationService.NavigateTo("Patient");
            }
        }
        public DoctorViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            Messenger.Default.Register<Doctor>(this, "Doctor", doctor =>
            {
                Doctor = doctor;
                RaisePropertyChanged(nameof(Doctor));
            });
            GetPatientCommand = new RelayCommand(async () =>
            {
                ObservableCollection<Pacient> tmp = null;
                do
                {
                    tmp = await DoctorUtilities.GetPatients(Doctor);
                } while (tmp == null);

                Patients = tmp;
                RaisePropertyChanged(nameof(Patients));
            });
            AddPatient = new RelayCommand(() =>
            {
                Messenger.Default.Send<Doctor>(Doctor, "Doctor");
                _navigationService.NavigateTo("NewPatient");
                Messenger.Default.Register<Pacient>(this, "Patient", pat =>
                {
                    Patients.Add(pat);
                });
            });
            DeletePatient = new RelayCommand(() =>
            {
                Tuple<Doctor, ObservableCollection<Pacient>> tup = new Tuple<Doctor, ObservableCollection<Pacient>>(Doctor, Patients);
                Messenger.Default.Send<ObservableCollection<Pacient>>(Patients, "DoctorPatients");
            });
        } 
        
        public RelayCommand GetPatientCommand { get; set; }
        public RelayCommand AddPatient { get; set; }
        public RelayCommand DeletePatient { get; set; }
    }
}
