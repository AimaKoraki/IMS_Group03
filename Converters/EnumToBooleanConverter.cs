// --- CORRECTED AND FINALIZED: Converters/EnumToBooleanConverter.cs ---
using System;
using System.Globalization;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts an enum value to a boolean. Returns true if the enum value matches the string parameter.
        /// </summary>
        /// <param name="value">The enum value from the binding source.</param>
        /// <param name="targetType">The type of the binding target property (always bool for a RadioButton).</param>
        /// <param name="parameter">The string name of the enum member to check against (e.g., "Pending").</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>True if the enum value's name matches the parameter; otherwise, false.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the bound value or the parameter is null, they can't match.
            if (value == null || parameter == null)
            {
                return false;
            }

            // Get the string representation of the enum value (e.g., "Pending")
            string enumValueString = value.ToString()!;
            // Get the string representation of the target enum member from the parameter
            string parameterString = parameter.ToString()!;

            // Compare the two strings, ignoring case for robustness.
            return enumValueString.Equals(parameterString, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Converts a boolean (from a RadioButton's IsChecked) back to an enum value.
        /// </summary>
        /// <param name="value">The boolean value from the binding target (the RadioButton).</param>
        /// <param name="targetType">The type of the binding source property (the enum type itself, e.g., typeof(OrderStatus)).</param>
        /// <param name="parameter">The string name of the enum member this RadioButton represents.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The enum value corresponding to the parameter if the RadioButton is checked; otherwise, Binding.DoNothing.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // We only care about updating the source if the RadioButton was CHECKED (true).
            // If it's being unchecked (false), we do nothing.
            if (value is not true || parameter == null)
            {
                return Binding.DoNothing;
            }

            // Get the string name of the enum member from the parameter.
            string parameterString = parameter.ToString()!;

            try
            {
                // Parse the string back into an actual enum value of the correct type.
                // For example, if targetType is typeof(OrderStatus) and parameterString is "Pending",
                // this will return OrderStatus.Pending.
                return Enum.Parse(targetType, parameterString, true);
            }
            catch (ArgumentException)
            {
                // This can happen if the XAML parameter is misspelled or not a valid member of the enum.
                // In this case, do nothing to prevent crashing the application.
                return Binding.DoNothing;
            }
        }
    }
}