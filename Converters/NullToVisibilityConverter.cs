// --- CORRECTED AND FINALIZED: Converters/NullToVisibilityConverter.cs ---
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    /// <summary>
    /// Converts a null or empty string value to a Visibility value.
    /// By default, returns Visible if the value is NOT null/empty, and Collapsed otherwise.
    /// Can be inverted with a parameter.
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // IMPROVEMENT: Use case-insensitive comparison for the parameter.
            bool invert = parameter is string stringParameter && stringParameter.Equals("invert", StringComparison.OrdinalIgnoreCase);

            // Your excellent logic is preserved here.
            // It's true if the value is null OR if it's a string that is null or empty.
            bool isNullOrEmpty = value == null || (value is string s && string.IsNullOrEmpty(s));

            if (invert)
            {
                // If inverted, show the element when the value IS null or empty.
                return isNullOrEmpty ? Visibility.Visible : Visibility.Collapsed;
            }

            // By default, show the element when the value is NOT null or empty.
            return !isNullOrEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // IMPROVEMENT: Use NotSupportedException for a one-way converter.
            throw new NotSupportedException("NullToVisibilityConverter cannot be used for two-way binding.");
        }
    }
}