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
    /// Логика взаимодействия для NewPatient.xaml
    /// </summary>
    public partial class NewPatient : Page
    {
        private NewPatientViewModel npvm => ViewModelLocator.NewPatient;
        public NewPatient()
        {
            InitializeComponent();
            DataContext = npvm;
        }

        private void AddPatient_OnClick(object sender, RoutedEventArgs e)
        {
           
            DateTime dt = DateTime.Parse(PatBirthdate.Text + "T00:00:00");
            dt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dt, TimeZoneInfo.Local.Id, "Russian Standard Time");
            Console.WriteLine(dt);
            Pacient pat = new Pacient
            {
                SNILS = PatSnils.Text,
                Name = PatName.Text,
                LastName = PatLastName.Text,
                Patronymic = PatPatronymic.Text,
                BirthdayDate = dt.Date,
                DeviceID = PatDevice.Text

            };
            npvm.AddPacient.Execute(pat);
        }
    }
}
