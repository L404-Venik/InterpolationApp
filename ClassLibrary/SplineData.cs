using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ClassLibrary
{
    
    public struct SplineDataItem
    {
        public double X;
        public double Y;
        public double dY;
        public double d2Y;
        public SplineDataItem(double x, double y, double dy, double d2Y)
        {
            this.X = x;
            this.Y = y;
            this.dY = dy;
            this.d2Y = d2Y;
        }
        public override string ToString()
        {
            return string.Format($"X = {X:f4}, F(x) = {Y:f4}, F'(x) = {dY:f4}, F''(x) = {d2Y:f4}");
        }
        public string ToString(string format)
        {
            return string.Format($"X = {X.ToString(format)}, F(x) = {Y.ToString(format)}, " +
                                 $"F'(x) = {dY.ToString(format)}, F''(x) = {d2Y.ToString(format)}");
        }
    }

    public class SplineData
    {
        public RawData Data { get; set; }
        public int NNodes { get; set; }
        public double[] BoundaryDerivatives { get; set; }
        public List<SplineDataItem> Items { get; set; }
        public double Integral { get; set; }
        public SplineData(RawData data, double[] Derivatives, int nNodes)
        {
            Data = data;
            BoundaryDerivatives = Derivatives;
            NNodes = nNodes;
            Integral = 0;
            this.Items = new List<SplineDataItem>();
        }

        [DllImport("..\\..\\..\\..\\x64\\Debug\\CPP_Lib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Call_MKL(
             double[] Nodes,
             int RDNNodes,
             bool Uniform,
             double[] Values,
             double[] Derivatives,
             int SDNNodes,
             double[] Ends,
             double[] SplineParameters,
             ref double Integral, 
             ref int ErrorType);
        public void Interpolate()
        {
            double[] SplineParameters = new double[3*NNodes] ;
            if (Items != null) 
                Items.Clear();
            
            double integral = 0;
            int ErrorType = 0;
            Call_MKL(
                Data.Uniform ? Data.Ends : Data.NodesArray, 
                Data.NNodes, 
                Data.Uniform, 
                Data.ValueArray, 
                BoundaryDerivatives, 
                NNodes,
                Data.Ends,
                SplineParameters,
                ref integral, 
                ref ErrorType);
            if (ErrorType != 0)
            {
                switch (ErrorType)
                {
                    case 1: throw new Exception("Ошибка создания задания");  
                    case 2: throw new Exception("Ошибка модификации параметров");
                    case 3: throw new Exception("Ошибка построения сплайна");
                    case 4: throw new Exception("Ошибка интерполяции");
                    case 5: throw new Exception("Ошибка интегрирования");
                }
                
            }

            Integral = integral;
            double step = (Data.Ends[1] - Data.Ends[0]) / (NNodes - 1);
            for (int i = 0; i < NNodes; i++)
            {
                double x = Data.Ends[0] + step * i;
                Items.Add(new SplineDataItem(x, SplineParameters[3 * i], SplineParameters[3 * i + 1], SplineParameters[3 * i + 2]));
            }
        }
    }
}
