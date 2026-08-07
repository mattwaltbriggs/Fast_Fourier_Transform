using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Fast_fourier_transform
{
    public partial class MainWindow : Window
    {
        public class TimeDomainEntry
        {
            public string Time { get; set; } = "";
            public string Amplitude { get; set; } = "";
        }

        public class FrequencyDomainEntry
        {
            public string Frequency { get; set; } = "";
            public string Magnitude { get; set; } = "";
            public string Phase { get; set; } = "";
            public string Real { get; set; } = "";
            public string Img { get; set; } = "";
        }

        private ObservableCollection<TimeDomainEntry> timeDomainData = new();
        private ObservableCollection<FrequencyDomainEntry> frequencyDomainData = new();

        public MainWindow()
        {
            InitializeComponent();
            TimeDomainGrid.ItemsSource = timeDomainData;
            FrequencyDomainGrid.ItemsSource = frequencyDomainData;
        }

        private void OnExitClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void OnImportClick(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null) return;

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Import Data",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Text files") { Patterns = new[] { "*.txt" } },
                    new FilePickerFileType("All files") { Patterns = new[] { "*.*" } }
                }
            });

            if (files.Count > 0)
            {
                try
                {
                    string text;
                    using (var stream = await files[0].OpenReadAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        text = await reader.ReadToEndAsync();
                    }
                    WriteDataToGrid(text);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Cannot read file: " + ex.Message);
                }
            }
        }

        private void OnPasteClick(object? sender, RoutedEventArgs e)
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard != null)
            {
                clipboard.TryGetTextAsync().ContinueWith(task =>
                {
                    var text = task.Result;
                    if (!string.IsNullOrEmpty(text))
                    {
                        Avalonia.Threading.Dispatcher.UIThread.Post(() => WriteDataToGrid(text));
                    }
                });
            }
        }

        private void OnClearDataClick(object? sender, RoutedEventArgs e)
        {
            timeDomainData.Clear();
        }

        private void OnTdChartClick(object? sender, RoutedEventArgs e)
        {
            if (timeDomainData.Count < 2) return;

            var xDatas = new List<double>();
            var yDatas = new List<double>();

            foreach (var item in timeDomainData)
            {
                if (co_functions.Test_a_textboxvalue_validity(item.Time, false, false) &&
                    co_functions.Test_a_textboxvalue_validity(item.Amplitude, false, false))
                {
                    if (double.TryParse(item.Time, out double timeValue) &&
                        double.TryParse(item.Amplitude, out double amplitudeValue))
                    {
                        xDatas.Add(timeValue);
                        yDatas.Add(amplitudeValue);
                    }
                }
            }

            if (xDatas.Count < 2) return;

            var cDataSeries = new ChartDataStore.DataSeriesStore(
                "Time domain values", 0, xDatas, yDatas,
                Avalonia.Media.Color.Parse("#8A2BE2"), 4, 1);

            var chartData = new ChartDataStore("Time Domain", "Time (s)", "Amplitude");
            chartData.AddDataSeries(cDataSeries);

            var chartWindow = new ChartViewWindow(chartData);
            chartWindow.Show();
        }

        private void OnFdChartClick(object? sender, RoutedEventArgs e)
        {
            if (frequencyDomainData.Count < 2) return;

            var fDatas = new List<double>();
            var magDatas = new List<double>();
            var phaseDatas = new List<double>();
            var realDatas = new List<double>();
            var imgDatas = new List<double>();

            foreach (var item in frequencyDomainData)
            {
                if (string.IsNullOrEmpty(item.Frequency) || string.IsNullOrEmpty(item.Magnitude) ||
                    string.IsNullOrEmpty(item.Phase) || string.IsNullOrEmpty(item.Real) ||
                    string.IsNullOrEmpty(item.Img))
                    continue;

                if (!co_functions.Test_a_textboxvalue_validity(item.Frequency, false, false) ||
                    !co_functions.Test_a_textboxvalue_validity(item.Magnitude, false, false) ||
                    !co_functions.Test_a_textboxvalue_validity(item.Phase, false, false) ||
                    !co_functions.Test_a_textboxvalue_validity(item.Real, false, false) ||
                    !co_functions.Test_a_textboxvalue_validity(item.Img, false, false))
                    continue;

                fDatas.Add(Convert.ToDouble(item.Frequency));
                magDatas.Add(Convert.ToDouble(item.Magnitude));
                phaseDatas.Add(Convert.ToDouble(item.Phase));
                realDatas.Add(Convert.ToDouble(item.Real));
                imgDatas.Add(Convert.ToDouble(item.Img));
            }

            var cDataSeries1 = new ChartDataStore.DataSeriesStore("Magnitude", 0, fDatas, magDatas,
                Avalonia.Media.Color.Parse("#008000"), 4, 2);
            var cDataSeries2 = new ChartDataStore.DataSeriesStore("Phase", 1, fDatas, phaseDatas,
                Avalonia.Media.Color.Parse("#9400D3"), 3, 1);
            var cDataSeries3 = new ChartDataStore.DataSeriesStore("Real", 2, fDatas, realDatas,
                Avalonia.Media.Color.Parse("#8B0000"), 4, 1);
            var cDataSeries4 = new ChartDataStore.DataSeriesStore("Img", 3, fDatas, imgDatas,
                Avalonia.Media.Color.Parse("#BC8F8F"), 4, 2);

            var chartData = new ChartDataStore("Frequency Domain", "Frequency (Hz)", "Magnitude/Phase/Real/Img");
            chartData.AddDataSeries(cDataSeries1);
            chartData.AddDataSeries(cDataSeries2);
            chartData.AddDataSeries(cDataSeries3);
            chartData.AddDataSeries(cDataSeries4);

            var chartWindow = new ChartViewWindow(chartData);
            chartWindow.Show();
        }

        private void OnFftClick(object? sender, RoutedEventArgs e)
        {
            if (timeDomainData.Count < 2) return;

            var xDatas = new List<double>();
            var yDatas = new List<double>();

            foreach (var item in timeDomainData)
            {
                if (co_functions.Test_a_textboxvalue_validity(item.Time, false, false) &&
                    co_functions.Test_a_textboxvalue_validity(item.Amplitude, false, false))
                {
                    if (double.TryParse(item.Time, out double timeValue) &&
                        double.TryParse(item.Amplitude, out double amplitudeValue))
                    {
                        xDatas.Add(timeValue);
                        yDatas.Add(amplitudeValue);
                    }
                }
            }

            var pd = new ProcessData(xDatas, yDatas);

            frequencyDomainData.Clear();
            for (int i = 0; i < pd.FftOutput.Count; i++)
            {
                frequencyDomainData.Add(new FrequencyDomainEntry
                {
                    Frequency = pd.FreqOutput[i].ToString(),
                    Magnitude = pd.FftOutput[i].GetMagnitude().ToString(),
                    Phase = pd.FftOutput[i].GetPhase().ToString(),
                    Real = pd.FftOutput[i].Real.ToString(),
                    Img = pd.FftOutput[i].Imag.ToString()
                });
            }
        }

        private void WriteDataToGrid(string s)
        {
            try
            {
                string[] lines = s.Replace("\n", "").Split('\r');

                if (lines.Length < 2) return;
                if (lines[0].Split('\t').Length != 2) return;

                timeDomainData.Clear();

                foreach (string item in lines)
                {
                    var fields = item.Split('\t');
                    if (fields.Length >= 2)
                    {
                        timeDomainData.Add(new TimeDomainEntry
                        {
                            Time = fields[0].Trim(),
                            Amplitude = fields[1].Trim()
                        });
                    }
                }
            }
            catch (FormatException)
            {
                return;
            }
        }
    }
}
