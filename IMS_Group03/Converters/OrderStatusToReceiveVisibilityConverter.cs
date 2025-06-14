// --- CORRECTED AND FINALIZED: Converters/OrderStatusToReceiveVisibilityConverter.cs ---
using IMS_Group03.Models; // For OrderStatus enum
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    /// <summary>
    /// A value converter that returns Visible if an OrderStatus is in a state where items can be received.
    /// Used to show a "Receive Items" button only for active orders.
    /// </summary>
    [ValueConversion(typeof(OrderStatus), typeof(Visibility))]
    public class OrderStatusToReceiveVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                // The business rule: A user can receive items for an order if it's
                // 'Pending', 'Processing', or has been 'Shipped'.
                bool canReceive = (status == OrderStatus.Pending ||
                                   status == OrderStatus.Processing ||
                                   status == OrderStatus.Shipped);

                return canReceive ? Visibility.Visible : Visibility.Collapsed;
            }

            // If the value is not a valid OrderStatus, hide the control for safety.
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // IMPROVEMENT: Use NotSupportedException for a one-way converter.
            throw new NotSupportedException("OrderStatusToReceiveVisibilityConverter cannot be used for two-way binding.");
        }
    }
}