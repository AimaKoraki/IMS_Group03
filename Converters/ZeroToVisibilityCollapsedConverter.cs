// --- COMPLETE AND FINALIZED: Converters/ZeroToVisibilityCollapsedConverter.cs ---
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    /// <summary>
    /// Converts an integer (e.g., a collection's Count) to a Visibility value.
    /// By default, it returns Visible if the value is > 0. If inverted, it returns Visible if the value is 0.
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class ZeroToVisibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int intValue)
            {
                // If the value is not an integer, default to collapsed for safety.
                return Visibility.Collapsed;
            }

            // Use case-insensitive comparison for the parameter.
            bool invert = parameter is string stringParameter && stringParameter.Equals("invert", StringComparison.OrdinalIgnoreCase);

            if (invert)
            {
                // Inverted logic is used for "No items found" messages.
                // Show the element only if the count is exactly 0.
                return intValue == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                // Default logic is used for the list/grid itself.
                // Show the element if the count is greater than 0.
                return intValue > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Use NotSupportedException for a one-way converter.
            throw new NotSupportedException("This converter cannot convert Visibility back to an integer.");
        }
    }
}