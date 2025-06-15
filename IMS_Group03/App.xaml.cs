// --- FULLY CORRECTED AND FINALIZED: App.xaml.cs ---
using IMS_Group03.Controllers;
using IMS_Group03.DataAccess;
using IMS_Group03.DataAccess.Repositories;
using IMS_Group03.Models;
using IMS_Group03.Services;
using IMS_Group03.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
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

                SeedDatabase(ServiceProvider);

                var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"A critical error occurred on startup: {ex.Message}\n\nCheck database connection and configuration.", "Fatal Error");
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
                MessageBox.Show("Database connection string is missing from appsettings.json.", "Configuration Error");
                Current.Shutdown();
                return;
            }

            // --- Database and Data Access Layer ---
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // --- Business Logic Services ---
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IStockMovementService, StockMovementService>();

            // --- Controllers ---
            // FIX: Register the Singleton MainController using an explicit factory.
            services.AddSingleton(provider => new MainController(provider));

            // Register all other controllers as Transient.
            services.AddTransient<LoginController>();
            services.AddTransient<DashboardController>();
            services.AddTransient<ProductController>();
            services.AddTransient<PurchaseOrderController>();
            services.AddTransient<SupplierController>();
            services.AddTransient<StockMovementController>();
            services.AddTransient<ReportController>();
            services.AddTransient<UserSettingsController>();

            // --- Windows and Views ---
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<DashboardView>();
            services.AddTransient<ProductView>();
            services.AddTransient<SupplierView>();
            services.AddTransient<PurchaseOrderView>();
            services.AddTransient<StockMovementView>();
            services.AddTransient<ReportView>();
            services.AddTransient<UserSettingsView>();

            // --- Global Converters ---
            services.AddSingleton<Converters.BooleanToVisibilityConverter>();
            services.AddSingleton<Converters.NullToVisibilityConverter>();
            // ... add other global converters here ...

            // --- Logging ---
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
        }

        private void SeedDatabase(IServiceProvider serviceProvider)
        {
#if DEBUG
            using (var scope = serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var adminExists = Task.Run(() => userService.GetUserByUsernameAsync("admin")).GetAwaiter().GetResult();

                if (adminExists == null)
                {
                    System.Diagnostics.Debug.WriteLine("--> No admin user found. Seeding initial admin user...");
                    var adminUser = new User
                    {
                        Username = "admin",
                        FullName = "Administrator",
                        Role = "Admin",
                        IsActive = true
                    };

                    var (success, _, message) = Task.Run(() => userService.CreateUserAsync(adminUser, "123")).GetAwaiter().GetResult();
                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine("--> Seeded initial admin user with password '123' successfully.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"--> FAILED to seed admin user: {message}");
                    }
                }
            }
#endif
        }
    }
}