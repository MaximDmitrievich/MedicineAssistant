using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using DoctorClientModule.ViewModel;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;

namespace DoctorClientModule
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherHelper.Initialize();
            ViewModelLocator.SetAndReg();
            ServiceLocator.Current.GetInstance<MainViewModel>();
        }
    }
}
