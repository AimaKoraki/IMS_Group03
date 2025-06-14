// --- FULLY CORRECTED AND FINALIZED: App.xaml.cs ---
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
// No need for System.Windows.Forms
using MessageBox = System.Windows.MessageBox;

namespace IMS_Group03
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public static IConfiguration Configuration { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            // This startup logic is already excellent and needs no changes.
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

                // This is the correct way to start the application.
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

            // --- FIX: The entire data access layer must share a scope to work correctly. ---
            // DbContext is registered as Scoped by default. This ensures that within one
            // scope, every class gets the *same* DbContext instance.
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // --- FIX: Repositories must be Scoped ---
            // This ensures they receive the same scoped DbContext as the UnitOfWork.
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // --- FIX: The Unit of Work must be Scoped ---
            // This is the orchestrator for a single, transactional operation.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // --- FIX: Services that use the Unit of Work must also be Scoped ---
            // This allows them to be part of the same transaction.
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IStockMovementService, StockMovementService>();

            // --- Controllers: Lifetimes are correct based on their role ---
            // MainController is a Singleton because it holds the global state of the application.
            services.AddSingleton<MainController>();

            // Other controllers are Transient because we want a fresh instance for each new view/operation.
            // They will be responsible for creating the scope for database work.
            services.AddTransient<LoginController>();
            services.AddTransient<DashboardController>();
            services.AddTransient<ProductController>();
            services.AddTransient<PurchaseOrderController>();
            services.AddTransient<SupplierController>();
            services.AddTransient<StockMovementController>();
            services.AddTransient<ReportController>();
            services.AddTransient<UserSettingsController>();

            // --- Windows and Views should always be Transient ---
            // We want a new window/view instance every time we open one.
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<DashboardView>();
            services.AddTransient<ProductView>();
            services.AddTransient<SupplierView>();
            services.AddTransient<PurchaseOrderView>();
            services.AddTransient<StockMovementView>();
            services.AddTransient<ReportView>();
            services.AddTransient<UserSettingsView>();

            // Logging setup is correct.
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