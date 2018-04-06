using DoctorClientModule.ViewModel;
using System.Windows;


namespace DoctorClientModule
{
    public partial class MainWindow : Window
    {
        private static MainViewModel Vm => ViewModelLocator.Main;

        public MainWindow()
        {
            InitializeComponent();
            //viewbox.Child = new Views.Doctor();
            DataContext = Vm;

        }
    }

}
