using ClassLibrary;
using Interface;
using System;
using System.ComponentModel;
using System.Windows;

namespace InterpolationApp
{
    class ViewData: IDataErrorInfo
    {
        public DataPlotModel Model { get; set; }
        public double[] Ends { get; set; }
        public int RDNNodes { get; set; }
        public bool Uniform { get; set; }
        public FRawEnum RDFunc { get; set; }
        public int SDNNodes { get; set; }
        public double[] Derivatives { get; set; }
        public RawData? rawData { get; set; }
        public SplineData? splineData { get; set; }
        public ViewData()
        {
            Ends = new double[2] { -1, 1 };
            RDNNodes = 5;
            SDNNodes = 5;
            Uniform = true;
            RDFunc = FRawEnum.Random;
            Derivatives = new double[2];
            rawData = new RawData(Ends, RDNNodes, Uniform, RawData.Random);
            splineData = new SplineData(rawData, Derivatives, SDNNodes);
        }
        public void Save(string file)
        {
            try
            {
                if (rawData != null)
                    rawData.Save(file);
                else
                    throw new Exception("rawData is null");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public void Load(string file)
        {
            try
            {
                rawData = new RawData(file);
                Ends = rawData.Ends;
                RDNNodes = rawData.NNodes;
                Uniform = rawData.Uniform;
                splineData = new SplineData(rawData, Derivatives, SDNNodes);

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        public override string ToString()
        {
            return $"LeftEnd = {Ends[0]}\n" +
                   $"RightEnd = {Ends[1]}\n" +
                   $"RawDataNNodes = {RDNNodes}\n" +
                   $"fRaw = {RDFunc}\n" +
                   $"Derivatives = {Derivatives[0]}, {Derivatives[1]}\n" +
                   $"Uniform = {Uniform}\n"+
                   $"SplineDataNNodes = {SDNNodes}\n";
        }

        public void Update()
        {
            if (Ends[0] > Ends[1])
                throw new Exception();
            if (RDFunc == FRawEnum.Random)
                rawData = new RawData(Ends, RDNNodes, Uniform, RawData.Random);
            else if (RDFunc == FRawEnum.Linear)
                rawData = new RawData(Ends, RDNNodes, Uniform, RawData.Linear);
            else
                rawData = new RawData(Ends, RDNNodes, Uniform, RawData.Cubic);
            splineData = new SplineData(rawData, Derivatives, SDNNodes);
        }
        public void Interpolate()
        {

            if (splineData == null)
                throw new Exception("no splinedata");
            splineData.Interpolate();
            
        }

        public void DrawPlot()
        {
            PlotData data = new PlotData();
            data.AddPoints(rawData, splineData.Items);

            Model = new DataPlotModel(data);
            Model.AddSeries();
        }
        #region Validaton
        string IDataErrorInfo.Error => "Error message";

        string IDataErrorInfo.this[string thing]
        {
            get
            {
                string message = "";
                if (RDNNodes <= 2)
                    message += "Количество узлов сетки должно быть больше 2";
                if (SDNNodes <= 2)
                    message += "Количество узлов интерполяции должно быть больше 2";
                if (Ends[0] >= Ends[1])
                    message += "Концы отрезка должны быть упорядочены";
                return message;
            }
        }
        #endregion
    }
}
