﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DoctorClientModule.Utilities;
using DoctorClientModule.ViewModel;

namespace DoctorClientModule.Views
{
    /// <summary>
    /// Логика взаимодействия для Patients.xaml
    /// </summary>
    public partial class Patient : Page
    {
        private static PatientViewModel Pvm => ViewModelLocator.Patient;
        public Patient()
        {
            InitializeComponent();
            DataContext = Pvm;
            Pvm.Start();
        }
        //override o
        ~Patient()
        {
            var t = 3;
        }
        
    }
}

