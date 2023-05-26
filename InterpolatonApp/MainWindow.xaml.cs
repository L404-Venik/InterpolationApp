using ClassLibrary;
using Interface;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InterpolationApp
{
    public partial class MainWindow : Window
    {
        ViewData VD = new ViewData();
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = VD;
            FuncSelect.ItemsSource = Enum.GetValues(typeof(FRawEnum));
        }

        private void Run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VD.Update();
                FillRawDataNodesListBox();
                VD.Interpolate(); 
                SplineDataInfo.ItemsSource = VD.splineData.Items;
                IntegralBlock.Text = VD.splineData.Integral.ToString();
                
                VD.DrawPlot();
                DataContext = VD.Model;
                DataContext = VD;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillRawDataNodesListBox()
        {
            RawDataInfo.Items.Clear();
            for (int i = 0; i < VD.RDNNodes; i++)
            {
                RawDataInfo.Items.Add($"X = {VD.rawData.NodesArray[i].ToString(VD.rawData.OutputFormat)}, Y = {VD.rawData.ValueArray[i].ToString(VD.rawData.OutputFormat)}");
            }
            RawDataInfo.Items.Refresh();
        }

        private void DataFromFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                if (fileDialog.ShowDialog() == true)
                {
                    VD.Load(fileDialog.FileName);
                }
                FillRawDataNodesListBox();
                VD.Interpolate();
                SplineDataInfo.ItemsSource = VD.splineData.Items;
                IntegralBlock.Text = VD.splineData.Integral.ToString();

                VD.DrawPlot();
                DataContext = VD.Model;
                DataContext = VD;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";
            if (saveFileDialog.ShowDialog() == true)
            {
                VD.Save(saveFileDialog.FileName);
            }
        }

        private void CanSaveCommandHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (RawDataNNodesField == null || LeftPointField == null || RightPointField == null)
            {
                e.CanExecute = false;
            }
            else
            if (Validation.GetHasError(RawDataNNodesField) == true || Validation.GetHasError(LeftPointField) == true || Validation.GetHasError(RightPointField) == true) 
            { 
                e.CanExecute = false;
            }
            else
                e.CanExecute = true;
        }

        public static RoutedCommand RunClickCommand =
            new RoutedCommand("RunClickCommand", typeof(MainWindow));
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(VD.ToString());
        }

        private void CanCalculateHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Validation.GetHasError(RawDataNNodesField)|| Validation.GetHasError(SplineDataNNodesField)||Validation.GetHasError(LeftPointField)||Validation.GetHasError(RightPointField)) 
            {
                e.CanExecute = false;
                return;
            }
            e.CanExecute = true;
        }
    }
}
