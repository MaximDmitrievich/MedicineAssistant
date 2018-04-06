using System.Windows.Documents;
using DoctorClientModule.DB;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using Newtonsoft.Json;
using System.Windows;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Navigation;
using System.Windows.Controls;
using DoctorClientModule.Navigation;

namespace DoctorClientModule.ViewModel
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private Page _page;
        public Page Win
        {
            get => _page;
            set
            {
                _page = value;
                RaisePropertyChanged();
            }
                
        }

        private readonly IFrameNavigationService _navigationService;
        public MainViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            Win = new Sign();
        }
        
    }
}