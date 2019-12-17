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
            var formattedTimeString = time.ToString("dd' 'MMM', 'H':'mm':'ss'.'fff");
            return formattedTimeString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
