// --- CORRECTED AND FINALIZED: Converters/CollectionToVisibilityConverter.cs ---
using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace IMS_Group03.Converters
{
    // IMPROVEMENT: Added ValueConversion attribute for tooling and framework hints.
    [ValueConversion(typeof(IEnumerable), typeof(Visibility))]
    public class CollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // IMPROVEMENT: Use case-insensitive comparison for the parameter.
            bool invert = parameter is string stringParameter && stringParameter.Equals("invert", StringComparison.OrdinalIgnoreCase);

            bool hasItems;

            if (value is ICollection collection)
            {
                // Most efficient check for collections with a Count property.
                hasItems = collection.Count > 0;
            }
            else if (value is IEnumerable enumerable)
            {
                // Fallback for any enumerable type (e.g., from a LINQ query).
                // This checks for the existence of at least one item without iterating the whole collection.
                hasItems = enumerable.GetEnumerator().MoveNext();
            }
            else
            {
                // If the value is null or not a collection, it has no items.
                hasItems = false;
            }

            if (invert)
            {
                // If inverted, visibility is true when there are NO items.
                return !hasItems ? Visibility.Visible : Visibility.Collapsed;
            }

            // By default, visibility is true when there ARE items.
            return hasItems ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // IMPROVEMENT: Use NotSupportedException for a one-way converter.
            throw new NotSupportedException("CollectionToVisibilityConverter cannot be used for two-way binding.");
        }
    }
}