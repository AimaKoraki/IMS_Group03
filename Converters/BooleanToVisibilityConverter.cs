// --- CORRECTED AND FINALIZED: Converters/BooleanToVisibilityConverter.cs ---
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a Visibility value.
        /// If the boolean is true, returns Visible; otherwise, returns Collapsed.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
            {
                // If the value is not a boolean, default to Collapsed for safety.
                return Visibility.Collapsed;
            }

            // Check if the parameter is a string and equals "invert" (case-insensitive)
            if (parameter is string stringParameter && stringParameter.Equals("invert", StringComparison.OrdinalIgnoreCase))
            {
                boolValue = !boolValue;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converting from Visibility back to a boolean is not supported.
        /// This prevents accidental use in two-way bindings.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("BooleanToVisibilityConverter cannot be used for two-way binding.");
        }
    }
}