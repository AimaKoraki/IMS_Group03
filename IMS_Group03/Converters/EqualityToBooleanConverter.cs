// --- NEW FILE: Converters/EqualityToBooleanConverter.cs ---
using System;
using System.Globalization;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    /// <summary>
    /// Converts a value to true if it equals the provided parameter.
    /// Used to check if a navigation button's identifier matches the current view's identifier.
    /// </summary>
    public class EqualityToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString()?.Equals(parameter?.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}