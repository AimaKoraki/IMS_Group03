// --- FULLY CORRECTED AND FINALIZED: Converters/ZeroToNormalRedConverter.cs ---
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace IMS_Group03.Converters
{
    /// <summary>
    /// Converts an integer value to a Brush. By default, returns an alert color (Red) if the value is
    /// greater than zero, and a normal theme color otherwise. Useful for KPI tiles.
    /// </summary>
    [ValueConversion(typeof(int), typeof(Brush))]
    public class ZeroToNormalRedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Define the "normal" color by trying to get a theme resource first, with a hardcoded fallback.
            var normalBrush = Application.Current.TryFindResource("MaterialDesignBody") as Brush ?? Brushes.Black;
            var alertBrush = Brushes.OrangeRed; // Define the alert color

            if (value is not int intValue)
            {
                // If the value is not an integer, just return the normal color.
                return normalBrush;
            }

            // Use case-insensitive comparison for the parameter.
            bool invert = parameter is string stringParameter && stringParameter.Equals("invert", StringComparison.OrdinalIgnoreCase);

            if (invert)
            {
                // Inverted logic: A value of 0 is the alert state.
                return intValue == 0 ? alertBrush : normalBrush;
            }

            // Default logic: A value greater than 0 is the alert state.
            // This is perfect for "Low Stock Items" or "Pending Orders" counts.
            return intValue > 0 ? alertBrush : normalBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Use NotSupportedException for a one-way converter.
            throw new NotSupportedException("This converter cannot convert a Brush back to an integer.");
        }
    }
}