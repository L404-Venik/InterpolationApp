using ClassLibrary;
using System.Collections.Generic;

namespace Interface
{
    public class PlotData
    {
        public List<double[]> XL { get; set; }
        public List<double[]> YL { get; set; }

        public List<string> Legends { get; set; }

        public PlotData()
        {
            Legends = new List<string>();
            XL = new List<double[]>();
            YL = new List<double[]>();
        }

        public void AddPoints(RawData rawData, List<SplineDataItem> items)
        {
            double[] X = new double[rawData.NNodes];
            double[] Y = new double[rawData.NNodes];
            for (int i = 0; i < rawData.NNodes; i++)
            {
                X[i] = rawData.NodesArray[i];
                Y[i] = rawData.ValueArray[i];
            }
            XL.Add(X);
            YL.Add(Y);
            Legends.Add($"RawData");

            X = new double[items.Count];
            Y = new double[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                X[i] = items[i].X;
                Y[i] = items[i].Y;
            }
            XL.Add(X);
            YL.Add(Y);
            Legends.Add($"SplineData");
        }
    }
}
