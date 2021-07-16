using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
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
        private static CancellationTokenSource source = new CancellationTokenSource();
        private static CancellationToken _token;

        public ObservableCollection<CardioPlot> CardioPlotLst { get; set; }
        public ObservableCollection<HeartRatePlot> HeartRatePlotLst { get; set; }
        public ObservableCollection<TemperaturePlot> TemperaturePlotLst { get; set; }
        public ObservableCollection<Tuple<DateTime, DateTime>> Ranges { get; set; }

        public string Emotion { get; set; }
        public string Disease { get; set; }


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
                if (_range != null)
                {
                    if (Equals(_range, Ranges?.FirstOrDefault()))
                    {
                        _timer.Start();
                    }
                    else
                    {
                        _timer.Stop();
                        PartPlot(_token);
                    }
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
                PhotoPath = "../Images/ic_account_circle_white_48dp_2x.png";
                RaisePropertyChanged(nameof(PhotoPath));
            });

            Back = new RelayCommand(async () =>
            {
                source.Cancel();
                await Task.Delay(500);
                _navigationService.GoBack();
            });

            CardioPlotLst = new ObservableCollection<CardioPlot>();
            HeartRatePlotLst = new ObservableCollection<HeartRatePlot>();
            TemperaturePlotLst = new ObservableCollection<TemperaturePlot>();

            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = TimeSpan.FromSeconds(1);
        }

        public RelayCommand Back { get; set; }
        
        private async void _timer_Tick(object sender, EventArgs e)
        {
            await RealTimePlot(_token);
        }

        public void Start()
        {
            source = new CancellationTokenSource();
            _token = source.Token;
            if (_token != null)
            {
                LoadRanges(_token);
            }
            
        }

        private async void LoadRanges(CancellationToken token)
        {
            
            if (Pacient != null && !token.IsCancellationRequested)
            {
                Ranges = new ObservableCollection<Tuple<DateTime, DateTime>>(await PatientUtilities.GetRanges(Pacient));
                if (Ranges?.Count > 0)
                {
                    RaisePropertyChanged(nameof(Ranges));
                    Range = new Tuple<DateTime, DateTime>(Ranges.FirstOrDefault().Item1, Ranges.FirstOrDefault().Item2);
                    _timer.Start();
                }
                
            }
        }

        private async Task RealTimePlot(CancellationToken token)
        {
             
            _timer.Stop();
            if (Pacient != null)
            {
                try
                {
                    if (!Equals(Range, null))
                    {
                        HumanData data = await PatientUtilities.LoadHumanData(Pacient, new Tuple<DateTime, DateTime>(Range.Item1, Range.Item2));
                        string dis = "Normal";
                        string emo = (await PatientUtilities.GetEmotion(Pacient))?.LastOrDefault()?.Emotion;
                        if (data != null && data.Data?.Count > 0)
                        {
                            Plot(data, token);
                            if (!Equals(Range, null))
                            {
                                Range = await PatientUtilities.GetLastRange(Pacient);
                                Disease = dis;
                                RaisePropertyChanged(nameof(Disease));
                                Emotion = emo;
                                RaisePropertyChanged(nameof(Emotion));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An unhandled exception just occurred: {ex.Message}", "Real Time Plot",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            _timer.Start();
        }

        private async void PartPlot(CancellationToken token)
        {
            _timer.Stop();
            if (Pacient != null)
            {
                try
                {
                    if (!Equals(Range, null))
                    {
                        HumanData data = await PatientUtilities.LoadHumanData(Pacient, new Tuple<DateTime, DateTime>(Range.Item1, Range.Item2));
                        string dis = "Normal";
                        string emo = (await PatientUtilities.GetEmotion(Pacient))?.LastOrDefault()?.Emotion;
                        if (data != null && data.Data?.Count > 0 && dis != null && emo != null)
                        {
                            Plot(data, token);
                            Disease = dis;
                            RaisePropertyChanged(nameof(Disease));
                            Emotion = emo;
                            RaisePropertyChanged(nameof(Emotion));
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

        private void Plot(HumanData hd, CancellationToken token)
        {
            CardioPlotLst.Clear();
            TemperaturePlotLst.Clear();
            HeartRatePlotLst.Clear();

            if (token.IsCancellationRequested)
            {
                _timer.Stop();
                Range = null;

                if (Ranges?.Count > 0)
                {
                    Ranges.Clear();
                }
                CardioPlotLst.Clear();
                RaisePropertyChanged(nameof(CardioPlotLst));
                TemperaturePlotLst.Clear();
                RaisePropertyChanged(nameof(TemperaturePlotLst));
                HeartRatePlotLst.Clear();
                RaisePropertyChanged(nameof(HeartRatePlotLst));
                HR = 0;
                return;
            }

            for (int i = 0; i < hd.Data.Count; i++)
            {
                
                for (int j = 0; j < hd.Data[i].Cardio.Count; j++)
                {
                    CardioPlotLst.Add(new CardioPlot
                    {
                        EventTime = PatientUtilities.UnixTimeStampToDateTime(hd.Data[i].Ticks[j]),
                        Cardio = hd.Data[i].Cardio[j]
                    });
                    RaisePropertyChanged(nameof(CardioPlotLst));
                    HeartRatePlotLst.Add(new HeartRatePlot
                    {
                        EventTime = PatientUtilities.UnixTimeStampToDateTime(hd.Data[i].Ticks[j]),
                        HeartRate = hd.Data[i].HR[j]
                    });
                    RaisePropertyChanged(nameof(HeartRatePlotLst));
                    TemperaturePlotLst.Add(new TemperaturePlot
                    {
                        EventTime = PatientUtilities.UnixTimeStampToDateTime(hd.Data[i].Ticks[j]),
                        Temperature = hd.Data[i].Temperature[j]
                    });
                    RaisePropertyChanged(nameof(TemperaturePlotLst));

                }
            }
            if (hd.Data?.Count > 0)
            {
                HR = hd.Data.LastOrDefault().HR.LastOrDefault();
            }
            RaisePropertyChanged(nameof(HR));
        }
    }
}