// --- CORRECTED AND FINALIZED: Converters/OrderStatusToCancelVisibilityConverter.cs ---
using IMS_Group03.Models; // For OrderStatus enum
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    /// <summary>
    /// A value converter that returns Visible if an OrderStatus is Pending or Processing.
    /// Used to show a "Cancel" button only for orders that are in a cancellable state.
    /// </summary>
    [ValueConversion(typeof(OrderStatus), typeof(Visibility))]
    public class OrderStatusToCancelVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                // The business rule: A user can only cancel an order if it is
                // currently 'Pending' or being 'Processing'.
                bool canCancel = (status == OrderStatus.Pending || status == OrderStatus.Processing);

                return canCancel ? Visibility.Visible : Visibility.Collapsed;
            }

            // If the value is not a valid OrderStatus, hide the control for safety.
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // IMPROVEMENT: Use NotSupportedException for a one-way converter.
            throw new NotSupportedException("OrderStatusToCancelVisibilityConverter cannot be used for two-way binding.");
        }
    }
}