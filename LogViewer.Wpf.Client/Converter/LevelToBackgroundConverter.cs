using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Serilog.Events;

namespace LogViewer.Wpf.Client.Converter
{
    internal class LevelToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var levelValue = (LogEventLevel) value;
            switch (levelValue)
            {
                case LogEventLevel.Debug:
                    return Brushes.DarkGray;
                case LogEventLevel.Verbose:
                    return Brushes.DimGray;
                case LogEventLevel.Information:
                    return Brushes.LightBlue;
                case LogEventLevel.Warning:
                    return Brushes.Coral;
                case LogEventLevel.Error:
                    return Brushes.IndianRed;
                case LogEventLevel.Fatal:
                    return Brushes.Red;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}