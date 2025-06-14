// --- MISSING FILE: Config/AppSettings.cs ---

namespace IMS_Group03.Config
{
    /// <summary>
    /// This class provides strongly-typed access to the "AppSettings" section
    /// of your appsettings.json file.
    /// </summary>
    public class AppSettings
    {
        public string ApplicationName { get; set; } = "Inventory Management System";
        public int DefaultLowStockThreshold { get; set; } = 10;
        public string ReportDateFormat { get; set; } = "yyyy-MM-dd HH:mm";
        public int MaxItemsPerPage { get; set; } = 25;
    }
}