using System;
using System.IO;

namespace ClassLibrary
{
    public enum FRawEnum { Random, Linear, Cubic };

    public delegate double FRaw(double x);
    public class RawData//нужен ли tostring?
    {
        public string OutputFormat = "f4";  // переменная для обозначения формата вывода чисел
        public double[] Ends { get; set; }
        public int NNodes { get; set; }
        public double step { get; set; }
        public bool Uniform { get; set; }
        public FRaw fRaw { get; set; }
        public double[] NodesArray { get; set; }
        public double[] ValueArray { get; set; }

        #region FRaws
        public static double Linear(double x)
        { return x; }
        public static double Cubic(double x)
        { return x * x * x; }
        public static double Random(double x)
        {
            Random y = new System.Random();
            return y.NextDouble();
        }
        #endregion

        public RawData(double[] Ends, int N, bool Unf, FRaw tp)
        {
            this.Ends = Ends;
            NNodes = N;
            Uniform = Unf;
            fRaw = tp;
            NodesArray = new double[NNodes];
            ValueArray = new double[NNodes];
            if (Uniform) { 
                if (N != 1)
                    step = (Ends[1] - Ends[0]) / (NNodes - 1);
                else
                    step = 0;
                for (int i = 0; i < NNodes; i++)
                {
                    NodesArray[i] = Ends[0] + (i * step);
                    ValueArray[i] = tp(NodesArray[i]);
                }
            }
            else
            {
                Random x = new Random();
                double range = Ends[1] - Ends[0];
                for (int i = 0; i < NNodes; i++)
                {
                    NodesArray[i] = Ends[0] + x.NextDouble() * range;
                    ValueArray[i] = tp(NodesArray[i]);
                }
                for(int i = 0; i < NNodes - 1; i++)
                {
                    for(int j = i+1; j < NNodes ; j++)
                    {
                        if (NodesArray[j] < NodesArray[i])
                        {
                            double buffer = NodesArray[j];
                            NodesArray[j] = NodesArray[i];
                            NodesArray[i] = buffer;
                        }
                    }
                }
                for (int i = 0; i < NNodes; i++)
                {
                    ValueArray[i] = tp(NodesArray[i]);
                }
            }
        }
        public RawData(double[] Ends, int N, bool Unf)
        {
            this.Ends = Ends;
            NNodes = N;
            Uniform = Unf;
            if (NNodes != 1)
                step = (Ends[1] - Ends[0]) / (NNodes - 1);
            else
                step = 0;
        }
        public RawData(string file)
        {
            try
            {
                using (FileStream filestream = new FileStream(file, FileMode.Open))
                {
                    using (var Br = new BinaryReader(filestream))
                    {
                        this.Ends = new double[2];
                        Ends[0] = Br.ReadDouble();
                        Ends[1] = Br.ReadDouble();
                        this.NNodes = Br.ReadInt32();
                        this.Uniform = Br.ReadBoolean();
                        NodesArray = new double[NNodes];
                        ValueArray = new double[NNodes];
                        for (int i = 0; i < NNodes; i++)
                        {
                            this.NodesArray[i] = Br.ReadDouble();
                            this.ValueArray[i] = Br.ReadDouble();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public bool Save(string file)
        {
            try
            {
                using (FileStream filestream = new FileStream(file, FileMode.Create))
                {
                    using (var BW = new BinaryWriter(filestream))
                    {
                        BW.Write(Ends[0]);
                        BW.Write(Ends[1]);
                        BW.Write(NNodes);
                        BW.Write(Uniform);
                        for (int i = 0; i < NNodes; i++)
                        {
                            BW.Write(NodesArray[i]);
                            BW.Write(ValueArray[i]);
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static void Load(string file, out RawData rawData)
        {
            rawData = new RawData(file);
        }
        public override string ToString()
        {
            string result = "";
            for(int i = 0; i < this.NNodes;i++)
            {
                result += $"x = {NodesArray[i].ToString(OutputFormat)},  y = {ValueArray[i].ToString(OutputFormat)};\n";
            }
            return result;
        }
    }
}
