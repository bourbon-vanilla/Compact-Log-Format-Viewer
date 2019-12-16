using System;
using System.Globalization;
using System.Windows.Data;

namespace LogViewer.Wpf.Client.Converter
{
    internal class TimeToFormattedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (DateTimeOffset)value;
            return time.ToString("yyyy'.'MM'.'dd' 'HH':'mm':'ss'.'fff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
