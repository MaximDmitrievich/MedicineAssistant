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

namespace DoctorClientModule
{
    /// <summary>
    /// Логика взаимодействия для Sign.xaml
    /// </summary>
    public partial class Sign : Page
    { 

        private static SignViewModel Svm => ViewModelLocator.Sign;
        public Sign()
        {
            InitializeComponent();
            ToSignIn.Visibility = Visibility.Visible;
            SignUp.Visibility = Visibility.Collapsed;
            DataContext = Svm;
            
        }

        private void SignUp_OnClick(object sender, RoutedEventArgs e)
        {
            Doctor doc = new Doctor
            {
                Id = null,
                Login = DocLogin.Text,
                Password = DocPass.Password,
                Name = DocName.Text,
                LastName = DocLastName.Text,
                Patronymic = DocPatronymic.Text,
                Specialization = DocSpecialization.Text

            };
            Svm.SignUp.Execute(doc);
            if (Svm.Doctor != null)
            {
                SignUp.Visibility = Visibility.Collapsed;
            }
        }

        private void ToSignUp_OnClick(object sender, RoutedEventArgs e)
        {
            ToSignIn.Visibility = Visibility.Collapsed;
            SignUp.Visibility = Visibility.Visible;
        }

        private void SignIn_OnClick(object sender, RoutedEventArgs e)
        {
            Tuple<string, string> logpas = new Tuple<string, string>(Log.Text, Pass.Password);
            Svm.SignIn.Execute(logpas);
            ToSignIn.Visibility = Visibility.Collapsed;
        }
    }
}
