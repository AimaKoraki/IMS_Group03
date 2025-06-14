// --- FULLY CORRECTED AND FINALIZED: Converters/InverseBooleanConverter.cs ---
using System;
using System.Globalization;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    /// <summary>
    /// A value converter that inverts a boolean value.
    /// True becomes False, and False becomes True.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter // FIX: Space added here
    {
        /// <summary>
        /// Converts a boolean to its opposite value.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            // If the input is not a boolean, do not apply a value.
            return Binding.DoNothing;
        }

        /// <summary>
        /// Converts the inverted boolean back to its original value.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // The operation is symmetrical, so the logic is the same.
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return Binding.DoNothing;
        }
    }
}