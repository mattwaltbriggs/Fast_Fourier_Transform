using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Fast_fourier_transform
{
    public partial class ChartViewWindow : Window
    {
        private ChartDataStore theChart;
        private ChartOptionWindow? chartOptionForm;

        public ChartDataStore TheChart => theChart;

        public ChartViewWindow(ChartDataStore chartData)
        {
            InitializeComponent();
            theChart = chartData;

            var chartCanvasControl = this.FindControl<ChartCanvas>("ChartCanvasControl")!;
            chartCanvasControl.ChartData = theChart;
            chartCanvasControl.InvalidateVisual();

            var dataSeriesMenu = this.FindControl<MenuItem>("DataSeriesMenu")!;
            foreach (var ds in theChart.DataSeriesList)
            {
                var item = new MenuItem { Header = ds.SeriesName, Tag = ds.MenuItemName };
                item.Click += OnDataSeriesClick;
                dataSeriesMenu.Items.Add(item);
            }
        }

        private void OnDataSeriesClick(object? sender, RoutedEventArgs e)
        {
            if (sender is MenuItem clickedItem && clickedItem.Tag is string tagName)
            {
                foreach (var ds in theChart.DataSeriesList)
                {
                    if (ds.MenuItemName == tagName)
                    {
                        ds.UpdateChartView(!ds.ShowChart);
                        var chartCanvasControl = this.FindControl<ChartCanvas>("ChartCanvasControl");
                        chartCanvasControl?.InvalidateVisual();
                        return;
                    }
                }
            }
        }

        private void OnFormatChartArea(object? sender, RoutedEventArgs e)
        {
            chartOptionForm = new ChartOptionWindow(this, theChart, 0);
            chartOptionForm.ShowDialog(this);
        }

        private void OnFormatHorizontalAxis(object? sender, RoutedEventArgs e)
        {
            chartOptionForm = new ChartOptionWindow(this, theChart, 1);
            chartOptionForm.ShowDialog(this);
        }

        private void OnFormatVerticalAxis(object? sender, RoutedEventArgs e)
        {
            chartOptionForm = new ChartOptionWindow(this, theChart, 2);
            chartOptionForm.ShowDialog(this);
        }

        private void OnFormatDataSeries(object? sender, RoutedEventArgs e)
        {
            chartOptionForm = new ChartOptionWindow(this, theChart, 3);
            chartOptionForm.ShowDialog(this);
        }

        public void RefreshChart()
        {
            var chartCanvasControl = this.FindControl<ChartCanvas>("ChartCanvasControl");
            chartCanvasControl?.InvalidateVisual();
        }
    }

    public class ChartCanvas : Control
    {
        public ChartDataStore? ChartData { get; set; }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (ChartData == null || Bounds.Width <= 0 || Bounds.Height <= 0) return;

            double margin_left = 80;
            double margin_right = 30;
            double margin_top = 50;
            double margin_bottom = 60;

            double originX = margin_left;
            double originY = margin_top;
            double scaleX = Bounds.Width - margin_left - margin_right;
            double scaleY = Bounds.Height - margin_top - margin_bottom;

            if (scaleX <= 0 || scaleY <= 0) return;

            using (context.PushTransform(Matrix.CreateTranslation(originX, originY)))
            {
                ChartData.PaintChart(context, scaleX, scaleY);
            }
        }
    }
}
