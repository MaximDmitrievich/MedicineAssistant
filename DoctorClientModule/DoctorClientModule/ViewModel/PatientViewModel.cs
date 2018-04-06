using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using DoctorClientModule.Charts;
using DoctorClientModule.DB;
using DoctorClientModule.Navigation;
using DoctorClientModule.Utilities;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;

namespace DoctorClientModule.Views
{
    public class PatientViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private IFrameNavigationService _navigationService;
        private Tuple<DateTime, DateTime> _range;
        private DispatcherTimer _timer;
        private static bool isEnded = true;

        public ObservableCollection<CardioPlot> CardioPlotLst { get; set; }
        public ObservableCollection<HeartRatePlot> HeartRatePlotLst { get; set; }
        public ObservableCollection<TemperaturePlot> TemperaturePlotLst { get; set; }
        public ObservableCollection<Tuple<DateTime, DateTime>> Ranges { get; set; }


        public Pacient Pacient { get; set; }
        public double HR { get; set; }
        public string PhotoPath { get; set; }
        public Tuple<DateTime, DateTime> Range
        {
            get { return _range; }
            set
            {
                _range = value;
                RaisePropertyChanged(nameof(Range));
                _timer.Stop();
                if (Equals(_range, Ranges?.FirstOrDefault()))
                {
                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                    PartPlot();
                }
            }
        }


        public PatientViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            Messenger.Default.Register<Pacient>(this, "Patient", pat =>
            {
                Pacient = pat;
                RaisePropertyChanged(nameof(Pacient));
                if (Pacient != null)
                {
                    if (Pacient.Name == "Maxim")
                    {
                        PhotoPath = "../Images/2.jpg";
                        RaisePropertyChanged(nameof(PhotoPath));
                    }
                    else if (Pacient.Name == "Maximilian")
                    {
                        PhotoPath = "../Images/3.jpg";
                        RaisePropertyChanged(nameof(PhotoPath));
                    }
                }
            });

            Back = new RelayCommand(async () =>
            {
                _timer.Stop();
                Range = null;
                if (Ranges?.Count > 0)
                {
                    Ranges.Clear();
                }
                CardioPlotLst.Clear();
                TemperaturePlotLst.Clear();
                HeartRatePlotLst.Clear();
                while (!isEnded)
                {
                    await Task.Delay(5);
                }
                _navigationService.GoBack();
            });


            CardioPlotLst = new ObservableCollection<CardioPlot>();
            HeartRatePlotLst = new ObservableCollection<HeartRatePlot>();
            TemperaturePlotLst = new ObservableCollection<TemperaturePlot>();

            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(5);
        }

        public RelayCommand Back { get; set; }
        
        private async void _timer_Tick(object sender, EventArgs e)
        {
            if (isEnded)
            {
                isEnded = false;
                await RealTimePlot();
                isEnded = true;
            }
        }

        public void Start()
        {
            LoadRanges();
        }

        private async void LoadRanges()
        {
            if (Pacient != null)
            {
                Ranges = new ObservableCollection<Tuple<DateTime, DateTime>>(await PatientUtilities.GetRanges(Pacient));
                if (Ranges?.Count > 0)
                {
                    RaisePropertyChanged(nameof(Ranges));
                    Range = new Tuple<DateTime, DateTime>(Ranges.FirstOrDefault().Item1, Ranges.FirstOrDefault().Item2);
                    await FirstPlot();
                }
                
            }
        }

        private async Task FirstPlot()
        {
            if (!Equals(Range, null))
            {
                HumanData data = await PatientUtilities.LoadHumanData(Pacient, Range);
                if (data != null && data.Data?.Count > 0)
                {
                    Plot(data);
                }
                _timer.Start();
            }
        }

        private async Task RealTimePlot()
        {
            if (Pacient != null)
            {
                try
                {
                    if (!Equals(Range, null))
                    {
                        HumanData data = await PatientUtilities.LoadHumanData(Pacient, new Tuple<DateTime, DateTime>(Range.Item1, Range.Item2));
                        if (data != null && data.Data?.Count > 0)
                        {
                            Plot(data);
                            if (!Equals(Range, null))
                            {
                                Range = new Tuple<DateTime, DateTime>(Range.Item2, DateTime.Now.ToUniversalTime());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "RealTimePlot",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void PartPlot()
        {
            if (Pacient != null)
            {
                try
                {
                    if (!Equals(Range, null))
                    {
                        HumanData data = await PatientUtilities.LoadHumanData(Pacient, new Tuple<DateTime, DateTime>(Range.Item1, Range.Item2));
                        if (data != null && data.Data?.Count > 0)
                        {
                            CardioPlotLst = new ObservableCollection<CardioPlot>();
                            HeartRatePlotLst = new ObservableCollection<HeartRatePlot>();
                            TemperaturePlotLst = new ObservableCollection<TemperaturePlot>();
                            GC.Collect();
                            Plot(data);
                        }
                    }
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Part Plot",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        /*
        private async Task Reload()
        {
            if (Pacient != null)
            {
                HumanData temp = null;
                try
                {
                    if (Equals(Range, Ranges.FirstOrDefault()))
                    {
                        DateTime now = DateTime.Now.ToUniversalTime();
                        //List<Tuple<DateTime, DateTime>> range = await PatientUtilities.GetRanges(Pacient);
                        temp = await PatientUtilities.LoadHumanData(Pacient, new Tuple<DateTime, DateTime>(LastTime, now));
                        if (temp != null && temp.Data?.Count > 0)
                        {
                                Range = new Tuple<DateTime, DateTime>(PatientUtilities.UnixTimeStampToDateTime(temp.Data.LastOrDefault().Ticks.LastOrDefault()), now);
                                CardioPlotLst = new ObservableCollection<CardioPlot>();
                                HeartRatePlotLst = new ObservableCollection<HeartRatePlot>();
                                TemperaturePlotLst = new ObservableCollection<TemperaturePlot>();
                                GC.Collect();
                                Plot(temp);
                        }
                    }
                    else
                    {
                        temp = await PatientUtilities.LoadHumanData(Pacient, new Tuple<DateTime, DateTime>(Range.Item1, Range.Item2));
                        Plot(temp);
                    }
                    

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Reload Data",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }*/

        private void Plot(HumanData hd)
        {
            for (int i = 0; i < hd.Data.Count; i++)
            {
                for (int j = 0; j < hd.Data[i].Cardio.Count; j++)
                {
                    CardioPlotLst.Add(new CardioPlot
                    {
                        EventTime = PatientUtilities.UnixTimeStampToDateTime(hd.Data[i].Ticks[j]),
                        Cardio = hd.Data[i].Cardio[j]
                    });
                    HeartRatePlotLst.Add(new HeartRatePlot
                    {
                        EventTime = PatientUtilities.UnixTimeStampToDateTime(hd.Data[i].Ticks[j]),
                        HeartRate = hd.Data[i].HR[j]
                    });
                    TemperaturePlotLst.Add(new TemperaturePlot
                    {
                        EventTime = PatientUtilities.UnixTimeStampToDateTime(hd.Data[i].Ticks[j]),
                        Temperature = hd.Data[i].Temperature[j]
                    });

                }
            }
            RaisePropertyChanged(nameof(CardioPlotLst));
            RaisePropertyChanged(nameof(HeartRatePlotLst));
            RaisePropertyChanged(nameof(TemperaturePlotLst));
            if (hd.Data?.Count > 0)
            {
                HR = hd.Data.LastOrDefault().HR.LastOrDefault();
            }
            RaisePropertyChanged(nameof(HR));
        }
    }
}