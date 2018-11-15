using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
namespace DataProcessing.Core
{
    public class ChartComponent
    {
        public static PlotModel BarPlot(string title, List<object> x, IEnumerable<double> y, string xLabel = "", string yLabel = "")
        {
            var model = new PlotModel { Title = "Class histogram" };

            var barSeries = new ColumnSeries();
            var bars = new List<ColumnItem>();
            for (int i = 0; i < x.Count; i++)
            {
                var b = new ColumnItem(y.ElementAt(i), i);
                bars.Add(b);
            }
            barSeries.ItemsSource = bars;
            //add category axis
            var xAxes = new CategoryAxis
            {
                Title = xLabel,
                Position = AxisPosition.Bottom,
                Key = "Target",
                ItemsSource = x.ToArray(),
            };
            xAxes.ActualLabels.AddRange(x.Select(xx => xx.ToString()));
            model.Axes.Add(xAxes);


            //add category axis
            var yAxes = new LinearAxis()
            {
                Title = yLabel,
                Position = AxisPosition.Left,


            };
            model.Axes.Add(yAxes);

            model.Series.Add(barSeries);



            //
            return model;

        }

        public static PlotModel ScaterPlot(string title, double[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType, string xLabel = " ", string yLabel = " ")
        {
            var model = new PlotModel { Title = "Class histogram" };
            var scater = ScatterSeries(x, y, markerColor, mType);

            model.Series.Add(scater);
            //add category axis
            var xAxes = new LinearAxis
            {
                Title = xLabel,
                Position = AxisPosition.Bottom,
            };
            model.Axes.Add(xAxes);


            //add category axis
            var yAxes = new LinearAxis()
            {
                Title = yLabel,
                Position = AxisPosition.Left,
            };
            model.Axes.Add(yAxes);
            //
            return model;
        }

        public static LineSeries LineSeries(string seriesTitle, double[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType)
        {
            var lineSeries = new LineSeries();
            lineSeries.Title = seriesTitle;
            for (int i = 0; i < x.Length; i++)
            {
                var b = new DataPoint(x[i], y[i]);

                lineSeries.Points.Add(b);
            }
            lineSeries.Color= OxyColor.FromRgb(markerColor.R, markerColor.G, markerColor.B);
            //lineSeries.MarkerFill = 
            lineSeries.MarkerType = mType;
            return lineSeries;
        }

        public static ScatterSeries ScatterSeries(double[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType)
        {
            var scatterSeries = new ScatterSeries();
            // var scatter = new List<DataPoint>();

            for (int i = 0; i < x.Length; i++)
            {
                var b = new ScatterPoint(x[i], y[i], 4.0, 1);

                scatterSeries.Points.Add(b);
            }
            scatterSeries.MarkerFill = OxyColor.FromRgb(markerColor.R, markerColor.G, markerColor.B);
            scatterSeries.MarkerType = mType;
            return scatterSeries;
        }

        public static PlotModel LinePlot(string titlePlot, string titleSeries, double[] x, double[] y, System.Drawing.Color markerColor, MarkerType mType, string xLabel = " ", string yLabel = " ")
        {
            var model = new PlotModel { Title = titlePlot };
            var scater = LineSeries(titleSeries, x, y, markerColor, mType);

            model.Series.Add(scater);
            //add category axis
            var xAxes = new LinearAxis
            {
                Title = xLabel,
                Position = AxisPosition.Bottom,
            };
            model.Axes.Add(xAxes);


            //add category axis
            var yAxes = new LinearAxis()
            {
                Title = yLabel,
                Position = AxisPosition.Left,
            };
            model.Axes.Add(yAxes);
            //
            return model;
        }
    }
}
