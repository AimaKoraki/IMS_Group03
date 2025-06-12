// App.xaml.cs
using IMS_Group03.Controllers;
using IMS_Group03.DataAccess;
using IMS_Group03.DataAccess.Repositories; // Assuming this is where your repository interfaces and classes are
using IMS_Group03.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks; // Not strictly needed here unless OnStartup becomes async for other reasons
using System.Windows;

namespace IMS_Group03
{
    public partial class App : Application
    {
        // Corrected static Configuration property
        public static IConfiguration Configuration { get; private set; } = null!; // Initialize with null-forgiving

        // ServiceProvider remains the same
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Or AppContext.BaseDirectory
                .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build(); // Assign to the static property

            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration); // Make IConfiguration instance available

            string? connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection string 'DefaultConnection' not found in appsettings.json. Application will exit.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown(1);
                return; // Stop further configuration
            }

            // Register DbContext ONCE
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            // Repositories (Ensure these interfaces and classes exist)
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<ISupplierRepository, SupplierRepository>();
            services.AddTransient<IPurchaseOrderRepository, PurchaseOrderRepository>();
            services.AddTransient<IStockMovementRepository, StockMovementRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            // services.AddTransient<IExpenseRepository, ExpenseRepository>();


            // Services (Ensure these interfaces and classes exist)
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<ISupplierService, SupplierService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IStockMovementService, StockMovementService>();
            services.AddTransient<IUserService, UserService>();
            // services.AddTransient<IAuthService, AuthService>();
            // services.AddTransient<IExpenseService, ExpenseService>();

            // Controllers
            services.AddTransient<MainController>();
            services.AddTransient<ProductController>();
            services.AddTransient<SupplierController>();
            services.AddTransient<PurchaseOrderController>();
            services.AddTransient<StockMovementController>();
            services.AddTransient<DashboardController>();
            services.AddTransient<ReportController>();
            services.AddTransient<UserSettingsController>();
            // services.AddTransient<ExpensesController>();


            // Windows & Views (Register if they have constructor dependencies or if you want to resolve them via DI)
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>(); // If LoginWindow needs services
            services.AddTransient<Views.DashboardView>();
            services.AddTransient<Views.ProductView>();
            services.AddTransient<Views.SupplierView>();
            services.AddTransient<Views.PurchaseOrderView>();
            services.AddTransient<Views.StockMovementView>();
            services.AddTransient<Views.ReportView>();
            services.AddTransient<Views.UserSettingsView>();
            // services.AddTransient<Views.ExpensesView>();

            // Logging
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // All DI setup is done in the constructor and ConfigureServices.
            // ServiceProvider is now built and ready.

            LoginWindow loginWindow = new LoginWindow(); // Create instance
            // If LoginWindow itself was registered and had dependencies:
            // LoginWindow loginWindow = ServiceProvider.GetService<LoginWindow>();
            // if (loginWindow == null) { /* Handle error */ Current.Shutdown(1); return; }

            loginWindow.Show();

            // MainWindow will be created and shown by LoginWindow after successful authentication.
            // LoginWindow will use App.ServiceProvider.GetService<MainWindow>() to create it,
            // ensuring all its dependencies (like MainController) are injected.
        }
    }
}