// --- FINAL GUARANTEED VERSION: App.xaml.cs ---
using IMS_Group03.Controllers;
using IMS_Group03.DataAccess;
using IMS_Group03.DataAccess.Repositories;
using IMS_Group03.Services;
using IMS_Group03.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace IMS_Group03
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true);
                Configuration = builder.Build();

                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                ServiceProvider = serviceCollection.BuildServiceProvider();

                // Use the hardcoded login window for our test
                var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A critical error occurred on startup: {ex.Message}", "Fatal Error");
                Current.Shutdown();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            services.AddSingleton(Configuration);
            string? connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection string is missing.", "Error");
                Current.Shutdown();
                return;
            }
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;
            services.AddTransient(x => new AppDbContext(dbContextOptions));

            // Repositories
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ISupplierRepository, SupplierRepository>();
            services.AddTransient<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddTransient<IStockMovementRepository, StockMovementRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            // Unit of Work
            // We use the version that receives repositories via its constructor.
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Services
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ISupplierService, SupplierService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IStockMovementService, StockMovementService>();

            // Controllers
            services.AddSingleton<MainController>();
            services.AddTransient<LoginController>();
            services.AddTransient<DashboardController>();
            services.AddTransient<ProductController>();
            services.AddTransient<PurchaseOrderController>();
            services.AddTransient<SupplierController>();
            services.AddTransient<StockMovementController>();
            services.AddTransient<ReportController>();
            services.AddTransient<UserSettingsController>();

            // Windows and Views
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<DashboardView>();
            services.AddTransient<ProductView>();
            services.AddTransient<SupplierView>();
            services.AddTransient<PurchaseOrderView>();
            services.AddTransient<StockMovementView>();
            services.AddTransient<ReportView>();
            services.AddTransient<UserSettingsView>();

            // Logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
        }
    }
}