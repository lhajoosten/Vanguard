using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vanguard.Persistence.Data;

namespace Vanguard.Infrastructure.Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Database Migrator Starting...");

            // Find the API project's directory to use its appsettings
            string apiProjectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "WebApi"));

            // Build configuration from API project
            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Setup DI
            var services = new ServiceCollection();

            // Add your DbContext
            services.AddDbContext<VanguardDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Get the DbContext
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<VanguardDbContext>();

                Console.WriteLine("Applying migrations...");
                dbContext.Database.Migrate();
                Console.WriteLine("Migrations applied successfully!");
            }

            Console.WriteLine("Database migration completed successfully.");
        }
    }
}