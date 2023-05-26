using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System.Linq;

namespace Interface
{
    public class DataPlotModel
    {
        public PlotData Data;
        public PlotModel PlotModel { get; private set; }

        public DataPlotModel(PlotData data)
        {
            Data = data;
            PlotModel = new PlotModel { Title = "" };
            AddSeries();
        }

        public void AddSeries()
        {
            PlotModel.Series.Clear();

            OxyColor[] colors = new OxyColor[2] { OxyColors.AliceBlue, OxyColors.Crimson };

            ScatterSeries scatterSeries = new ScatterSeries();
            for (int i = 0; i < Data.XL[0].Length; i++)
                scatterSeries.Points.Add(new ScatterPoint(Data.XL[0][i], Data.YL[0][i]));
            scatterSeries.MarkerType = MarkerType.Diamond;
            scatterSeries.MarkerSize = 4;
            scatterSeries.MarkerStroke = OxyColors.DarkCyan;
            scatterSeries.MarkerFill = colors[0];
            scatterSeries.Title = Data.Legends[0];

            LineSeries lineSeries = new LineSeries();
            for (int i = 0; i < Data.XL[1].Length; i++)
                lineSeries.Points.Add(new DataPoint(Data.XL[1][i], Data.YL[1][i]));
            lineSeries.Color = colors[1];
            lineSeries.MarkerType = MarkerType.Circle;
            lineSeries.MarkerSize = 3;
            lineSeries.MarkerStroke = colors[1];
            lineSeries.MarkerFill = colors[1];
            lineSeries.MarkerStrokeThickness = 1;
            lineSeries.Title = Data.Legends[1];

            Legend legend = new Legend { LegendPosition = LegendPosition.TopLeft };
            PlotModel.Legends.Add(legend);

            PlotModel.Series.Add(lineSeries);
            PlotModel.Series.Add(scatterSeries);
            

        }

    }
}
