// --- CORRECTED AND FINALIZED: Converters/StringFormatConverter.cs ---
using System;
using System.Globalization;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    /// <summary>
    /// A flexible converter to format strings. Uses the ConverterParameter as the format string.
    /// Special case: If parameter is "N/A_IfNullOrEmpty", it returns "N/A" for null or empty strings.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Get the format string from the ConverterParameter in XAML.
            string? format = parameter as string;

            // Handle the special case first.
            if (format == "N/A_IfNullOrEmpty")
            {
                return string.IsNullOrEmpty(value as string) ? "N/A" : value;
            }

            // Handle the general formatting case.
            if (!string.IsNullOrEmpty(format) && value != null)
            {
                // Uses standard .NET string formatting, e.g., "{0:C}" for currency, "{0:d}" for short date.
                return string.Format(culture, format, value);
            }

            // If no format is provided, just return the value's string representation or an empty string for null.
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // IMPROVEMENT: Use NotSupportedException for a one-way converter.
            throw new NotSupportedException("StringFormatConverter cannot be used for two-way binding.");
        }
    }
}