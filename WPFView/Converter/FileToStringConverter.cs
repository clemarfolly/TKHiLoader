using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace TKHiLoader.Converter
{
    public class FileToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ret = string.Empty;

            string file = value as string;

            if (!string.IsNullOrWhiteSpace(file))
            {
                ret = File.ReadAllText(file);
            }

            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}