// --- FILE: Config/AppSettings.cs ---

namespace IMS_Group03.Config
{
    /// <summary>
    /// Provides strongly-typed access to application settings, typically bound 
    /// from the "AppSettings" section of an appsettings.json file.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the name of the application. This is often used for display
        /// in UI elements like window titles or page headers.
        /// </summary>
        /// <remarks>
        /// Default value is "Inventory Management System".
        /// </remarks>
        public string ApplicationName { get; set; } = "Inventory Management System";

        /// <summary>
        /// Gets or sets the stock level at which an item is considered "low".
        /// This threshold is used to trigger alerts or for reporting purposes.
        /// </summary>
        /// <remarks>
        /// Default value is 10.
        /// </remarks>
        public int DefaultLowStockThreshold { get; set; } = 10;

        /// <summary>
        /// Gets or sets the standard date and time format string for generated reports.
        /// Uses standard .NET format specifiers.
        /// </summary>
        /// <example>
        /// A format of "yyyy-MM-dd HH:mm" would produce output like "2023-10-27 14:30".
        /// </example>
        /// <remarks>
        /// Default value is "yyyy-MM-dd HH:mm".
        /// </remarks>
        public string ReportDateFormat { get; set; } = "yyyy-MM-dd HH:mm";

        /// <summary>
        /// Gets or sets the maximum number of items to display on a single page
        /// in paginated lists (e.g., a product catalog or order history).
        /// </summary>
        /// <remarks>
        /// Default value is 25.
        /// </remarks>
        public int MaxItemsPerPage { get; set; } = 25;
    }
}