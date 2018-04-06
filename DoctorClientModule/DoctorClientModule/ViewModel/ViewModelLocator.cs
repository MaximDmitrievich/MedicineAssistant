using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using CommonServiceLocator;
using DoctorClientModule.Navigation;
using System;
using DoctorClientModule.Views;

namespace DoctorClientModule.ViewModel
{
    public class ViewModelLocator 
    {
        public ViewModelLocator()
        {
            //SetAndReg();
           
        }

        internal static void SetAndReg()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var navigationService = new FrameNavigationService();
            navigationService.Configure("Sign", new Uri("Views/Sign.xaml", UriKind.Relative));
            navigationService.Configure("Doctor", new Uri("Views/Doctor.xaml", UriKind.Relative));
            navigationService.Configure("Patient", new Uri("Views/Patient.xaml", UriKind.Relative));
            navigationService.Configure("NewPatient", new Uri("Views/NewPatient.xaml", UriKind.Relative));
            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SignViewModel>();
            SimpleIoc.Default.Register<PatientViewModel>(true);
            SimpleIoc.Default.Register<DoctorViewModel>(true);
            SimpleIoc.Default.Register<NewPatientViewModel>(true);
        }

        public static PatientViewModel Patient => ServiceLocator.Current.GetInstance<PatientViewModel>();

        public static DoctorViewModel Doctor => ServiceLocator.Current.GetInstance<DoctorViewModel>();

        public static SignViewModel Sign => ServiceLocator.Current.GetInstance<SignViewModel>();

        public static MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public static NewPatientViewModel NewPatient => ServiceLocator.Current.GetInstance<NewPatientViewModel>();
        
        public static void Cleanup()
        {

        }
    }
}