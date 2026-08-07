using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;

namespace Fast_fourier_transform
{
    public class ChartDataStore
    {
        private Color axisColor;
        private bool paintChartTitle = true;
        private string chartTitle;

        private double horizontalBoundWidth;
        private double horizontalBoundsMaximum;
        private double horizontalBoundsMinimum;
        private double horizontalUnitsMajor;
        private double horizontalUnitsMinor;
        private bool showHorizontalMajorGridLine = true;
        private bool showHorizontalMinorGridLine = false;
        private string horizontalAxisTitle;

        private double verticalBoundHeight;
        private double verticalBoundsMaximum;
        private double verticalBoundsMinimum;
        private double verticalUnitsMajor;
        private double verticalUnitsMinor;
        private bool showVerticalMajorGridLine = true;
        private bool showVerticalMinorGridLine = false;
        private string verticalAxisTitle;

        private double dataMaxX;
        private double dataMaxY;
        private double dataMinX;
        private double dataMinY;

        private bool chartXFormatAutomatic;
        private bool chartYFormatAutomatic;

        public string ChartTitle => chartTitle;
        public Color AxisColor => axisColor;
        public bool PaintChartTitle => paintChartTitle;
        public string HorizontalAxisTitle => horizontalAxisTitle;
        public double HorizontalBoundsMaximum => horizontalBoundsMaximum;
        public double HorizontalBoundsMinimum => horizontalBoundsMinimum;
        public double HorizontalUnitsMajor => horizontalUnitsMajor;
        public double HorizontalUnitsMinor => horizontalUnitsMinor;
        public bool ShowHorizontalMajorGridLine => showHorizontalMajorGridLine;
        public bool ShowHorizontalMinorGridLine => showHorizontalMinorGridLine;
        public bool ChartXFormatAutomatic => chartXFormatAutomatic;
        public string VerticalAxisTitle => verticalAxisTitle;
        public double VerticalBoundsMaximum => verticalBoundsMaximum;
        public double VerticalBoundsMinimum => verticalBoundsMinimum;
        public double VerticalUnitsMajor => verticalUnitsMajor;
        public double VerticalUnitsMinor => verticalUnitsMinor;
        public bool ShowVerticalMajorGridLine => showVerticalMajorGridLine;
        public bool ShowVerticalMinorGridLine => showVerticalMinorGridLine;
        public bool ChartYFormatAutomatic => chartYFormatAutomatic;
        public List<DataSeriesStore> DataSeriesList { get; } = new List<DataSeriesStore>();

        public ChartDataStore(string chartTitle = "Chart Title", string horizontalAxisTitle = "X Axis", string verticalAxisTitle = "Y Axis")
        {
            this.chartTitle = chartTitle;
            this.horizontalAxisTitle = horizontalAxisTitle;
            this.verticalAxisTitle = verticalAxisTitle;
            axisColor = Color.Parse("#0066CC");
            chartXFormatAutomatic = true;
            chartYFormatAutomatic = true;
        }

        public void FormatChartArea(string chartTitle, bool paintChartTitle, Color axisColor)
        {
            this.chartTitle = chartTitle;
            this.paintChartTitle = paintChartTitle;
            this.axisColor = axisColor;
        }

        public void FormatHorizontalAxis(string horizAxisTitle, double horizMaxBounds, double horizMinBounds,
            double horizMajorUnits, double horizMinorUnits, bool showMajorUnits, bool showMinorUnits, bool isAutomatic)
        {
            horizontalAxisTitle = horizAxisTitle;
            showHorizontalMajorGridLine = showMajorUnits;
            showHorizontalMinorGridLine = showMinorUnits;

            if (!isAutomatic)
            {
                chartXFormatAutomatic = false;
                horizontalBoundsMaximum = horizMaxBounds;
                horizontalBoundsMinimum = horizMinBounds;
                horizontalBoundWidth = horizontalBoundsMaximum - horizontalBoundsMinimum;
                horizontalUnitsMajor = horizMajorUnits;
                horizontalUnitsMinor = horizMinorUnits;
            }
            else
            {
                chartXFormatAutomatic = true;
                FormatChartAxis(dataMaxX, dataMinX, ref horizontalBoundWidth, ref horizontalBoundsMaximum,
                    ref horizontalBoundsMinimum, ref horizontalUnitsMajor, ref horizontalUnitsMinor);
            }
        }

        public void FormatVerticalAxis(string vertAxisTitle, double vertMaxBounds, double vertMinBounds,
            double vertMajorUnits, double vertMinorUnits, bool showMajorUnits, bool showMinorUnits, bool isAutomatic)
        {
            verticalAxisTitle = vertAxisTitle;
            showVerticalMajorGridLine = showMajorUnits;
            showVerticalMinorGridLine = showMinorUnits;

            if (!isAutomatic)
            {
                chartYFormatAutomatic = false;
                verticalBoundsMaximum = vertMaxBounds;
                verticalBoundsMinimum = vertMinBounds;
                verticalBoundHeight = verticalBoundsMaximum - verticalBoundsMinimum;
                verticalUnitsMajor = vertMajorUnits;
                verticalUnitsMinor = vertMinorUnits;
            }
            else
            {
                chartYFormatAutomatic = true;
                FormatChartAxis(dataMaxY, dataMinY, ref verticalBoundHeight, ref verticalBoundsMaximum,
                    ref verticalBoundsMinimum, ref verticalUnitsMajor, ref verticalUnitsMinor);
            }
        }

        public void FormatDataSeries(int index, string name, Color color, int chartType, int markerType, int lineWidth, int linePattern)
        {
            DataSeriesList[index].FormatDataSeries(name, color, chartType, markerType, lineWidth, linePattern);
        }

        public void AddDataSeries(DataSeriesStore dataSeries)
        {
            DataSeriesList.Add(dataSeries);
            SetMaxMin();

            if (chartXFormatAutomatic)
                FormatChartAxis(dataMaxX, dataMinX, ref horizontalBoundWidth, ref horizontalBoundsMaximum,
                    ref horizontalBoundsMinimum, ref horizontalUnitsMajor, ref horizontalUnitsMinor);

            if (chartYFormatAutomatic)
                FormatChartAxis(dataMaxY, dataMinY, ref verticalBoundHeight, ref verticalBoundsMaximum,
                    ref verticalBoundsMinimum, ref verticalUnitsMajor, ref verticalUnitsMinor);
        }

        private void SetMaxMin()
        {
            dataMaxX = double.MinValue;
            dataMaxY = double.MinValue;
            dataMinX = double.MaxValue;
            dataMinY = double.MaxValue;

            foreach (var ds in DataSeriesList)
            {
                dataMaxX = Math.Max(ds.MaxX, dataMaxX);
                dataMinX = Math.Min(ds.MinX, dataMinX);
                dataMaxY = Math.Max(ds.MaxY, dataMaxY);
                dataMinY = Math.Min(ds.MinY, dataMinY);
            }
        }

        private void FormatChartAxis(double dMax, double dMin, ref double boundDist, ref double boundsMaximum,
            ref double boundsMinimum, ref double unitsMajor, ref double unitsMinor)
        {
            double dPower, dScale, dSmall;

            if (dMax == dMin)
            {
                dMax = dMax * 1.01;
                dMin = dMin * 0.99;
            }

            if (dMax > 0)
                dMax = dMax + ((dMax - dMin) * 0.01);
            else if (dMax < 0)
                dMax = Math.Max(dMax + (dMax - dMin) * 0.01, 0);
            else
                dMax = 0;

            if (dMin > 0)
                dMin = Math.Max(dMin - (dMax - dMin) * 0.01, 0);
            else if (dMin < 0)
                dMin = dMin - ((dMax - dMin) * 0.01);
            else
                dMin = 0;

            if (dMax == 0 && dMin == 0)
                dMax = 1;

            dPower = Math.Log(dMax - dMin, 10) / Math.Log(10, 10);
            dScale = Math.Pow(10, (dPower - Convert.ToInt32(dPower)));

            if (dScale > 0 && dScale < 2.5) { dScale = 0.2; dSmall = 0.05; }
            else if (dScale > 2.5 && dScale < 5) { dScale = 0.5; dSmall = 0.1; }
            else if (dScale > 5 && dScale < 7.5) { dScale = 1; dSmall = 0.2; }
            else { dScale = 2; dSmall = 0.5; }

            dScale = dScale * Math.Pow(10, Convert.ToInt32(dPower));
            dSmall = dSmall * Math.Pow(10, Convert.ToInt32(dPower));

            boundsMinimum = dScale * (Convert.ToInt32(dMin / dScale) - 1);
            boundsMaximum = dScale * (Convert.ToInt32(dMax / dScale) + 1);
            unitsMinor = dSmall;
            unitsMajor = dScale;
            boundDist = boundsMaximum - boundsMinimum;
        }

        private static string FormatAxisValue(double value)
        {
            if (Math.Abs(value) < 1e-10) return "0";
            if (Math.Abs(value) >= 1000) return value.ToString("G4");
            if (Math.Abs(value) >= 1) return value.ToString("G4");
            return value.ToString("G3");
        }

        /// <summary>
        /// Paint the chart. Origin (0,0) is at the top-left of the chart area.
        /// X increases to the right, Y increases downward (Avalonia native).
        /// scaleX = chart width, scaleY = chart height (positive values).
        /// </summary>
        public void PaintChart(DrawingContext dc, double scaleX, double scaleY)
        {
            var axisBrush = new SolidColorBrush(axisColor);
            var faintBrush = new SolidColorBrush(Color.FromArgb(100,
                (byte)Math.Max(0, axisColor.R - 15),
                (byte)Math.Max(0, axisColor.G - 15),
                (byte)Math.Max(0, axisColor.B - 15)));

            if (paintChartTitle && scaleX > 0 && scaleY > 0)
            {
                var titleText = new FormattedText(chartTitle, System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, new Typeface("Cambria"), 20, axisBrush);
                dc.DrawText(titleText, new Point(scaleX * 0.5 - titleText.Width / 2, -35));
            }

            double tempVar;

            // ---- Horizontal axis (bottom edge at y = scaleY) ----
            // Horizontal minor grid lines
            if (showHorizontalMinorGridLine)
            {
                for (double hx = horizontalBoundsMinimum; hx <= horizontalBoundsMaximum; hx += horizontalUnitsMinor)
                {
                    tempVar = (hx - horizontalBoundsMinimum) / horizontalBoundWidth;
                    dc.DrawLine(new Pen(faintBrush),
                        new Point(tempVar * scaleX, 0),
                        new Point(tempVar * scaleX, scaleY));
                }
            }

            // Horizontal major grid lines + tick marks + labels
            for (double hx = horizontalBoundsMinimum; hx <= horizontalBoundsMaximum; hx += horizontalUnitsMajor)
            {
                tempVar = (hx - horizontalBoundsMinimum) / horizontalBoundWidth;
                if (showHorizontalMajorGridLine)
                {
                    dc.DrawLine(new Pen(axisBrush),
                        new Point(tempVar * scaleX, 0),
                        new Point(tempVar * scaleX, scaleY));
                }
                // Tick mark (downward from bottom axis)
                dc.DrawLine(new Pen(axisBrush),
                    new Point(tempVar * scaleX, scaleY),
                    new Point(tempVar * scaleX, scaleY + 10));

                var marking = new FormattedText(FormatAxisValue(hx), System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, new Typeface("Cambria"), 10, axisBrush);
                dc.DrawText(marking, new Point(tempVar * scaleX - marking.Width / 2, scaleY + 14));
            }

            // Horizontal axis title
            var hTitle = new FormattedText(horizontalAxisTitle, System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Cambria"), 14, axisBrush);
            dc.DrawText(hTitle, new Point(scaleX * 0.5 - hTitle.Width / 2, scaleY + 35));

            // ---- Vertical axis (left edge at x = 0) ----
            // Vertical minor grid lines
            if (showVerticalMinorGridLine)
            {
                for (double vy = verticalBoundsMinimum; vy <= verticalBoundsMaximum; vy += verticalUnitsMinor)
                {
                    tempVar = (vy - verticalBoundsMinimum) / verticalBoundHeight;
                    // Map: vy=min -> screen y=scaleY (bottom), vy=max -> screen y=0 (top)
                    double screenY = scaleY - tempVar * scaleY;
                    dc.DrawLine(new Pen(faintBrush),
                        new Point(0, screenY),
                        new Point(scaleX, screenY));
                }
            }

            // Vertical major grid lines + tick marks + labels
            for (double vy = verticalBoundsMinimum; vy <= verticalBoundsMaximum; vy += verticalUnitsMajor)
            {
                tempVar = (vy - verticalBoundsMinimum) / verticalBoundHeight;
                double screenY = scaleY - tempVar * scaleY;

                if (showVerticalMajorGridLine)
                {
                    dc.DrawLine(new Pen(axisBrush),
                        new Point(0, screenY),
                        new Point(scaleX, screenY));
                }
                // Tick mark (leftward from left axis)
                dc.DrawLine(new Pen(axisBrush),
                    new Point(0, screenY),
                    new Point(-10, screenY));

                var vMarking = new FormattedText(FormatAxisValue(vy), System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight, new Typeface("Cambria"), 10, axisBrush);

                // Rotate -90 degrees around the label center to read bottom-to-top
                var labelCenter = new Point(-20, screenY);
                using (dc.PushTransform(new RotateTransform(-90, labelCenter.X, labelCenter.Y).Value))
                {
                    dc.DrawText(vMarking, new Point(labelCenter.X - vMarking.Width / 2, labelCenter.Y - vMarking.Height / 2));
                }
            }

            // Vertical axis title
            var vTitle = new FormattedText(verticalAxisTitle, System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Cambria"), 14, axisBrush);
            var vTitleCenter = new Point(-50, scaleY * 0.5);
            using (dc.PushTransform(new RotateTransform(-90, vTitleCenter.X, vTitleCenter.Y).Value))
            {
                dc.DrawText(vTitle, new Point(vTitleCenter.X - vTitle.Width / 2, vTitleCenter.Y - vTitle.Height / 2));
            }

            // Paint data series
            foreach (var ds in DataSeriesList)
            {
                if (ds.ShowChart)
                {
                    ds.PaintChart(dc, scaleX, scaleY,
                        horizontalBoundsMaximum, horizontalBoundsMinimum,
                        verticalBoundsMaximum, verticalBoundsMinimum);
                }
            }
        }

        public class DataSeriesStore
        {
            private int uniqueId;
            private bool showChart;
            private int chartType;
            private int markerType;
            private int linePatternType;
            private float[] patternSet = new float[] { 3 };
            private int markerSize;
            private int lineWidth;
            private Color chartColor;
            private List<double> xVals;
            private List<double> yVals;
            private string seriesName;

            public double MaxX { get; }
            public double MaxY { get; }
            public double MinX { get; }
            public double MinY { get; }
            public string MenuItemName => "data_series" + uniqueId;
            public bool ShowChart => showChart;
            public string SeriesName => seriesName;
            public Color ChartColor => chartColor;
            public int ChartType => chartType;
            public int MarkerType => markerType;
            public int LineWidth => lineWidth;
            public int LinePattern => linePatternType;

            public DataSeriesStore(string seriesName, int uniqId, List<double> xVals, List<double> yVals,
                Color chartColor, int chartType = 4, int markerType = 3, int markerSize = 4,
                int lineWidth = 2, int linePattern = 0)
            {
                uniqueId = uniqId;
                showChart = true;
                this.seriesName = seriesName;
                this.xVals = new List<double>(xVals);
                this.yVals = new List<double>(yVals);
                this.chartColor = chartColor;
                this.chartType = chartType;
                this.markerType = markerType;
                this.markerSize = markerSize;
                this.lineWidth = lineWidth;
                linePatternType = linePattern;

                MaxX = this.xVals.Max();
                MinX = this.xVals.Min();
                MaxY = this.yVals.Max();
                MinY = this.yVals.Min();
            }

            public void UpdateChartView(bool show)
            {
                showChart = show;
            }

            public void FormatDataSeries(string name, Color color, int type, int marker, int width, int pattern)
            {
                seriesName = name;
                chartColor = color;
                chartType = type;
                markerType = marker;
                lineWidth = width;
                linePatternType = pattern;

                if (linePatternType == 0) patternSet = new float[] { 1 };
                else if (linePatternType == 1) patternSet = new float[] { 3, 1 };
                else if (linePatternType == 2) patternSet = new float[] { 4, 1 };
                else if (linePatternType == 3) patternSet = new float[] { 1, 3 };
                else if (linePatternType == 4) patternSet = new float[] { 1, 1 };
                else if (linePatternType == 5) patternSet = new float[] { 3, 1, 1, 1 };
                else if (linePatternType == 6) patternSet = new float[] { 4, 2, 2, 2 };
            }

            /// <summary>
            /// Paint data points. Origin at top-left, X right, Y down (Avalonia native).
            /// Data min maps to bottom (scaleY), data max maps to top (0).
            /// </summary>
            public void PaintChart(DrawingContext dc, double scaleX, double scaleY,
                double horizMax, double horizMin, double vertMax, double vertMin)
            {
                int varCount = Math.Min(xVals.Count, yVals.Count);
                var allPts = new List<List<Point>>();
                int segK = -1;
                bool segBreak = true;
                var brush = new SolidColorBrush(chartColor);

                for (int i = 0; i < varCount; i++)
                {
                    if (xVals[i] < horizMin || xVals[i] > horizMax || yVals[i] < vertMin || yVals[i] > vertMax)
                    {
                        segBreak = true;
                        continue;
                    }
                    else if (segBreak)
                    {
                        segBreak = false;
                        segK++;
                        if (segK >= 100) return;
                        allPts.Add(new List<Point>());
                    }

                    double tempX = (xVals[i] - horizMin) / (horizMax - horizMin);
                    double tempY = (yVals[i] - vertMin) / (vertMax - vertMin);
                    // Map to screen: x goes left-to-right, y goes top-to-bottom (invert data y)
                    var pt = new Point(tempX * scaleX, scaleY - tempY * scaleY);
                    allPts[segK].Add(pt);

                    if (chartType == 0 || chartType == 2 || chartType == 4)
                        PaintMarker(dc, pt, brush);
                }

                var pen = new Pen(brush, lineWidth);

                if (chartType == 1 || chartType == 2)
                {
                    for (int i = 0; i <= segK; i++)
                    {
                        if (allPts[i].Count >= 2)
                            dc.DrawGeometry(null, pen, StreamGeometryHelper.CreateLine(allPts[i]));
                    }
                }
                else if (chartType == 3 || chartType == 4)
                {
                    for (int i = 0; i <= segK; i++)
                    {
                        if (allPts[i].Count >= 2)
                        {
                            var geometry = StreamGeometryHelper.CreateSmoothLine(allPts[i]);
                            dc.DrawGeometry(null, pen, geometry);
                        }
                    }
                }
            }

            private void PaintMarker(DrawingContext dc, Point pt, IBrush brush)
            {
                double s = markerSize;
                switch (markerType)
                {
                    case 0: // Circle
                        dc.DrawEllipse(null, new Pen(brush), pt, s, s);
                        break;
                    case 1: // Filled circle
                        dc.DrawEllipse(brush, null, pt, s, s);
                        break;
                    case 2: // Rectangle
                        dc.DrawRectangle(null, new Pen(brush), new Rect(pt.X - s, pt.Y - s, 2 * s, 2 * s));
                        break;
                    case 3: // Filled rectangle
                        dc.DrawRectangle(brush, null, new Rect(pt.X - s, pt.Y - s, 2 * s, 2 * s));
                        break;
                    case 4: // Triangle
                    case 5: // Filled triangle
                        PaintTriangle(dc, pt, s, markerType == 5, brush);
                        break;
                }
            }

            private void PaintTriangle(DrawingContext dc, Point pt, double s, bool fill, IBrush brush)
            {
                double side = s * Math.Sqrt(3);
                var p0 = new Point(pt.X, pt.Y - (Math.Sqrt(3) / 3) * side);
                var p1 = new Point(pt.X - side / 2, pt.Y + (Math.Sqrt(3) / 6) * side);
                var p2 = new Point(pt.X + side / 2, pt.Y + (Math.Sqrt(3) / 6) * side);

                var geo = StreamGeometryHelper.CreateTriangle(p0, p1, p2);
                dc.DrawGeometry(fill ? brush : null, new Pen(brush), geo);
            }
        }
    }

    public static class StreamGeometryHelper
    {
        public static StreamGeometry CreateLine(List<Point> points)
        {
            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                ctx.BeginFigure(points[0], false);
                for (int i = 1; i < points.Count; i++)
                    ctx.LineTo(points[i]);
            }
            return geo;
        }

        public static StreamGeometry CreateSmoothLine(List<Point> points)
        {
            if (points.Count < 2) return new StreamGeometry();

            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                ctx.BeginFigure(points[0], false);

                for (int i = 1; i < points.Count; i++)
                {
                    var prev = points[i - 1];
                    var curr = points[i];

                    double dx = (curr.X - prev.X) / 3.0;
                    var cp1 = new Point(prev.X + dx, prev.Y);
                    var cp2 = new Point(curr.X - dx, curr.Y);

                    ctx.CubicBezierTo(cp1, cp2, curr);
                }
            }
            return geo;
        }

        public static StreamGeometry CreateTriangle(Point p0, Point p1, Point p2)
        {
            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                ctx.BeginFigure(p0, true);
                ctx.LineTo(p1);
                ctx.LineTo(p2);
                ctx.EndFigure(true);
            }
            return geo;
        }
    }
}
