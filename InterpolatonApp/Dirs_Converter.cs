using System;
using System.Windows.Data;

namespace InterpolationApp
{
    class Dirs_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                double[] doubles = value as double[];
                string strings = doubles[0].ToString() + ';' + doubles[1].ToString() ;
                return strings;
            }
            catch (Exception ex) { return " ; "; }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                string[] strings = ((string)value).Split(';');
                double[] doubles = new double[2];
                doubles[0] = System.Convert.ToDouble(strings[0]);
                doubles[1] = System.Convert.ToDouble(strings[1]);
                return doubles;
            }
            catch (Exception ex) { return new object[2]; }
        }
    }
}
