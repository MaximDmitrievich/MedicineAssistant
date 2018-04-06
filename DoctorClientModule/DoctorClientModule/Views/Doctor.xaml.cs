using System;
using System.Collections.Generic;
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
using DoctorClientModule.DB;
using DoctorClientModule.ViewModel;

namespace DoctorClientModule.Views
{
    /// <summary>
    /// Логика взаимодействия для Doctor.xaml
    /// </summary>
    public partial class Doctor : Page
    {
        private static DoctorViewModel Dvm => ViewModelLocator.Doctor;
        public Doctor()
        {
            InitializeComponent();
            DataContext = Dvm;
            Dvm.GetPatientCommand.Execute(null);
        }

        private void AddPatient_OnClick(object sender, RoutedEventArgs e)
        {
            Dvm.AddPatient.Execute(null);
        }

        private void DeletePatient_OnClick(object sender, RoutedEventArgs e)
        {
            Dvm.DeletePatient.Execute(null);
        }
    }
}
