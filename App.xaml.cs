// --- FULLY CORRECTED AND FINALIZED: App.xaml.cs ---
using IMS_Group03.Controllers;
using IMS_Group03.DataAccess;
using IMS_Group03.DataAccess.Repositories;
using IMS_Group03.Models; // Required for User model in SeedDatabase
using IMS_Group03.Services;
using IMS_Group03.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks; // Required for Task.Run
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

                // Seed the database with the first user if necessary.
                // This is done BEFORE showing any window.
                SeedDatabase(ServiceProvider);

                // --- This is the correct startup sequence ---
                // Show the LoginWindow first. The LoginWindow itself will be responsible
                // for showing the MainWindow after a successful login.
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

            // DbContext is Scoped by default with AddDbContext.
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Scoped Data Access Layer
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Scoped Business Logic Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IStockMovementService, StockMovementService>();

            // Singleton Global Controller
            services.AddSingleton<MainController>();

            // Transient Page/View Controllers
            services.AddTransient<DashboardController>();
            services.AddTransient<ProductController>();
            services.AddTransient<PurchaseOrderController>();
            services.AddTransient<SupplierController>();
            services.AddTransient<StockMovementController>();
            services.AddTransient<ReportController>();
            services.AddTransient<UserSettingsController>();

            // --- FIX: Add LoginController registration ---
            services.AddTransient<LoginController>();

            // Transient Windows and Views
            services.AddTransient<MainWindow>();
            services.AddTransient<DashboardView>();
            services.AddTransient<ProductView>();
            services.AddTransient<SupplierView>();
            services.AddTransient<PurchaseOrderView>();
            services.AddTransient<StockMovementView>();
            services.AddTransient<ReportView>();
            services.AddTransient<UserSettingsView>();

            // --- FIX: Add LoginWindow registration ---
            services.AddTransient<LoginWindow>();

            // Logging setup
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
        }

        /// <summary>
        /// Seeds the database with an initial admin user if one does not exist.
        /// This method should only run in a development environment.
        /// </summary>
        private void SeedDatabase(IServiceProvider serviceProvider)
        {
#if DEBUG // This preprocessor directive ensures this code is only compiled in Debug builds.
            using (var scope = serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

                // We use GetAwaiter().GetResult() to run the async method synchronously on startup.
                // This is one of the few places where blocking is acceptable because the app can't start without this check.
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

                    // Create the user with a default password.
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