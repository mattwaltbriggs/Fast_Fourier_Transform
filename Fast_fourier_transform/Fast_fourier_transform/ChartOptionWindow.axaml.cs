using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace Fast_fourier_transform
{
    public partial class ChartOptionWindow : Window
    {
        private ChartViewWindow? parentForm;
        private ChartDataStore theChart;
        private int tabId;

        public ChartOptionWindow(ChartViewWindow parent, ChartDataStore chart, int tabId)
        {
            InitializeComponent();
            parentForm = parent;
            theChart = chart;
            this.tabId = tabId;
            LoadFormData();
        }

        private void LoadFormData()
        {
            switch (tabId)
            {
                case 0:
                    TabControlChart.Items.RemoveAt(3); // data series
                    TabControlChart.Items.RemoveAt(2); // vertical axis
                    TabControlChart.Items.RemoveAt(1); // horizontal axis
                    TextBoxChartTitle.Text = theChart.ChartTitle;
                    ButtonAxisColor.Background = new SolidColorBrush(theChart.AxisColor);
                    CheckBoxShowChartTitle.IsChecked = theChart.PaintChartTitle;
                    break;

                case 1:
                    TabControlChart.Items.RemoveAt(3);
                    TabControlChart.Items.RemoveAt(2);
                    TabControlChart.Items.RemoveAt(0);
                    TextBoxHAxisTitle.Text = theChart.HorizontalAxisTitle;
                    TextBoxHMaxBounds.Text = theChart.HorizontalBoundsMaximum.ToString();
                    TextBoxHMinBounds.Text = theChart.HorizontalBoundsMinimum.ToString();
                    TextBoxHMajorUnits.Text = theChart.HorizontalUnitsMajor.ToString();
                    TextBoxHMinorUnits.Text = theChart.HorizontalUnitsMinor.ToString();
                    CheckBoxHMajorGridlines.IsChecked = theChart.ShowHorizontalMajorGridLine;
                    CheckBoxHMinorGridlines.IsChecked = theChart.ShowHorizontalMinorGridLine;
                    if (theChart.ChartXFormatAutomatic)
                        RadioButtonHAutomatic.IsChecked = true;
                    else
                        RadioButtonHAxisValue.IsChecked = true;
                    UpdateHAutomaticOptionControl();
                    break;

                case 2:
                    TabControlChart.Items.RemoveAt(3);
                    TabControlChart.Items.RemoveAt(1);
                    TabControlChart.Items.RemoveAt(0);
                    TextBoxVAxisTitle.Text = theChart.VerticalAxisTitle;
                    TextBoxVMaxBounds.Text = theChart.VerticalBoundsMaximum.ToString();
                    TextBoxVMinBounds.Text = theChart.VerticalBoundsMinimum.ToString();
                    TextBoxVMajorUnits.Text = theChart.VerticalUnitsMajor.ToString();
                    TextBoxVMinorUnits.Text = theChart.VerticalUnitsMinor.ToString();
                    CheckBoxVMajorGridlines.IsChecked = theChart.ShowVerticalMajorGridLine;
                    CheckBoxVMinorGridlines.IsChecked = theChart.ShowVerticalMinorGridLine;
                    if (theChart.ChartYFormatAutomatic)
                        RadioButtonVAutomatic.IsChecked = true;
                    else
                        RadioButtonVAxisValue.IsChecked = true;
                    UpdateVAutomaticOptionControl();
                    break;

                case 3:
                    TabControlChart.Items.RemoveAt(2);
                    TabControlChart.Items.RemoveAt(1);
                    TabControlChart.Items.RemoveAt(0);
                    foreach (var ds in theChart.DataSeriesList)
                        ComboBoxDataSeriesList.Items.Add(ds.SeriesName);
                    ComboBoxDataSeriesList.SelectedIndex = 0;
                    SetDataSeriesList();
                    break;
            }
        }

        private void OnUpdateClick(object? sender, RoutedEventArgs e)
        {
            switch (tabId)
            {
                case 0:
                    theChart.FormatChartArea(TextBoxChartTitle.Text,
                        CheckBoxShowChartTitle.IsChecked ?? false,
                        ToColor(ButtonAxisColor.Background));
                    break;

                case 1:
                    if (RadioButtonHAutomatic.IsChecked == true)
                    {
                        theChart.FormatHorizontalAxis(TextBoxHAxisTitle.Text, 0, 0, 0, 0,
                            CheckBoxHMajorGridlines.IsChecked ?? false,
                            CheckBoxHMinorGridlines.IsChecked ?? false, true);
                    }
                    else
                    {
                        if (co_functions.Test_a_textboxvalue_validity(TextBoxHMaxBounds.Text, false, false) &&
                            co_functions.Test_a_textboxvalue_validity(TextBoxHMinBounds.Text, false, false) &&
                            co_functions.Test_a_textboxvalue_validity(TextBoxHMajorUnits.Text, false, false) &&
                            co_functions.Test_a_textboxvalue_validity(TextBoxHMinorUnits.Text, false, false))
                        {
                            double tMax = Convert.ToDouble(TextBoxHMaxBounds.Text);
                            double tMin = Convert.ToDouble(TextBoxHMinBounds.Text);
                            double tMajor = Convert.ToDouble(TextBoxHMajorUnits.Text);
                            double tMinor = Convert.ToDouble(TextBoxHMinorUnits.Text);

                            if (tMax > tMin && tMajor > tMinor)
                            {
                                theChart.FormatHorizontalAxis(TextBoxHAxisTitle.Text, tMax, tMin, tMajor, tMinor,
                                    CheckBoxHMajorGridlines.IsChecked ?? false,
                                    CheckBoxHMinorGridlines.IsChecked ?? false, false);
                            }
                        }
                    }
                    break;

                case 2:
                    if (RadioButtonVAutomatic.IsChecked == true)
                    {
                        theChart.FormatVerticalAxis(TextBoxVAxisTitle.Text, 0, 0, 0, 0,
                            CheckBoxVMajorGridlines.IsChecked ?? false,
                            CheckBoxVMinorGridlines.IsChecked ?? false, true);
                    }
                    else
                    {
                        if (co_functions.Test_a_textboxvalue_validity(TextBoxVMaxBounds.Text, false, false) &&
                            co_functions.Test_a_textboxvalue_validity(TextBoxVMinBounds.Text, false, false) &&
                            co_functions.Test_a_textboxvalue_validity(TextBoxVMajorUnits.Text, false, false) &&
                            co_functions.Test_a_textboxvalue_validity(TextBoxVMinorUnits.Text, false, false))
                        {
                            double tMax = Convert.ToDouble(TextBoxVMaxBounds.Text);
                            double tMin = Convert.ToDouble(TextBoxVMinBounds.Text);
                            double tMajor = Convert.ToDouble(TextBoxVMajorUnits.Text);
                            double tMinor = Convert.ToDouble(TextBoxVMinorUnits.Text);

                            if (tMax > tMin && tMajor > tMinor)
                            {
                                theChart.FormatVerticalAxis(TextBoxVAxisTitle.Text, tMax, tMin, tMajor, tMinor,
                                    CheckBoxVMajorGridlines.IsChecked ?? false,
                                    CheckBoxVMinorGridlines.IsChecked ?? false, false);
                            }
                        }
                    }
                    break;

                case 3:
                    int sIndex = ComboBoxDataSeriesList.SelectedIndex;
                    theChart.FormatDataSeries(sIndex, TextBoxDataSeriesTitle.Text,
                        ToColor(ButtonDataColor.Background),
                        ComboBoxChartType.SelectedIndex,
                        ComboBoxMarkerType.SelectedIndex,
                        ComboBoxLineWidth.SelectedIndex + 1,
                        ComboBoxLinePattern.SelectedIndex);
                    SetDataSeriesList();
                    break;
            }

            parentForm?.RefreshChart();
        }

        private void OnCloseClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnAxisColorClick(object? sender, RoutedEventArgs e)
        {
            ButtonAxisColor.Background = new SolidColorBrush(Color.Parse("#FF0000"));
        }

        private void OnDataColorClick(object? sender, RoutedEventArgs e)
        {
            ButtonDataColor.Background = new SolidColorBrush(Color.Parse("#FF0000"));
        }

        private void OnHAutomaticChanged(object? sender, RoutedEventArgs e) => UpdateHAutomaticOptionControl();
        private void OnHAxisValueChanged(object? sender, RoutedEventArgs e) => UpdateHAutomaticOptionControl();
        private void OnVAutomaticChanged(object? sender, RoutedEventArgs e) => UpdateVAutomaticOptionControl();
        private void OnVAxisValueChanged(object? sender, RoutedEventArgs e) => UpdateVAutomaticOptionControl();

        private void UpdateHAutomaticOptionControl()
        {
            bool isAuto = RadioButtonHAutomatic.IsChecked == true;
            TextBoxHMaxBounds.IsEnabled = !isAuto;
            TextBoxHMinBounds.IsEnabled = !isAuto;
            TextBoxHMajorUnits.IsEnabled = !isAuto;
            TextBoxHMinorUnits.IsEnabled = !isAuto;
        }

        private void UpdateVAutomaticOptionControl()
        {
            bool isAuto = RadioButtonVAutomatic.IsChecked == true;
            TextBoxVMaxBounds.IsEnabled = !isAuto;
            TextBoxVMinBounds.IsEnabled = !isAuto;
            TextBoxVMajorUnits.IsEnabled = !isAuto;
            TextBoxVMinorUnits.IsEnabled = !isAuto;
        }

        private void OnDataSeriesListChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (ComboBoxDataSeriesList.SelectedIndex >= 0)
                SetDataSeriesList();
        }

        private void SetDataSeriesList()
        {
            int idx = ComboBoxDataSeriesList.SelectedIndex;
            if (idx < 0 || idx >= theChart.DataSeriesList.Count) return;

            var ds = theChart.DataSeriesList[idx];
            TextBoxDataSeriesTitle.Text = ds.SeriesName;
            ButtonDataColor.Background = new SolidColorBrush(ds.ChartColor);
            ComboBoxChartType.SelectedIndex = ds.ChartType;
            ComboBoxMarkerType.SelectedIndex = ds.MarkerType;
            ComboBoxLineWidth.SelectedIndex = ds.LineWidth - 1;
            ComboBoxLinePattern.SelectedIndex = ds.LinePattern;
        }

        private static Color ToColor(IBrush brush)
        {
            if (brush is SolidColorBrush sb)
                return sb.Color;
            return Color.Parse("#0066CC");
        }
    }
}
