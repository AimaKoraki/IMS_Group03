// File: DataAccess/DesignTimeDbContextFactory.cs (or similar)
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace IMS_Group03.DataAccess 
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Get the current directory. This is usually the project directory when tools are run.
            string basePath = Directory.GetCurrentDirectory();
            string appSettingsInConfigPath = Path.Combine(basePath, "Config", "appsettings.json");
            string appSettingsAtRootPath = Path.Combine(basePath, "appsettings.json");

            // Output paths for debugging if needed
            // Console.WriteLine($"Trying appsettings.json path (in Config): {appSettingsInConfigPath}");
            // Console.WriteLine($"Trying appsettings.json path (at root): {appSettingsAtRootPath}");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath); // Base path is the project directory

            // Check if appsettings.json exists in Config folder
            if (File.Exists(appSettingsInConfigPath))
            {
                configurationBuilder.AddJsonFile(Path.Combine("Config", "appsettings.json"), optional: false, reloadOnChange: true);
                // Console.WriteLine("Found appsettings.json in Config folder.");
            }
            // Else, check if it exists at the project root (less likely given your structure)
            else if (File.Exists(appSettingsAtRootPath))
            {
                configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                // Console.WriteLine("Found appsettings.json in project root.");
            }
            else
            {
                throw new FileNotFoundException(
                    $"appsettings.json not found. Checked paths: {appSettingsInConfigPath} and {appSettingsAtRootPath}. " +
                    $"Current directory for tools: {basePath}. Ensure 'Copy to Output Directory' is set for appsettings.json if it's not found at these relative paths during design time."
                );
            }

            IConfigurationRoot configuration = configurationBuilder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "DesignTimeDbContextFactory: Connection string 'DefaultConnection' not found in appsettings.json. " +
                    "Ensure it's present and the file was loaded correctly."
                );
            }

            // Configure it to use SQL Server (or MySQL if you switched back)
            optionsBuilder.UseSqlServer(connectionString);
            // For MySQL: optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}