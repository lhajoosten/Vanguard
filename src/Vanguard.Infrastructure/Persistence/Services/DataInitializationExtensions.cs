using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Vanguard.Infrastructure.Persistence.Context;

namespace Vanguard.Infrastructure.Persistence.Services
{
    public static class DataInitializationExtensions
    {
        public static async Task InitializeDatabaseAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<VanguardContext>>();

            try
            {
                logger.LogInformation("Initializing database");

                var context = services.GetRequiredService<VanguardContext>();

                // Ensure database is created and migrations are applied
                await context.Database.MigrateAsync();

                // Seed enumeration data
                var enumSeeder = services.GetRequiredService<EnumDataSeeder>();
                await enumSeeder.SeedAsync(context);

                // Seed custom data
                var customSeeder = services.GetRequiredService<CustomDataSeeder>();
                await customSeeder.SeedAsync(context);

                logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database");
                throw;
            }
        }

        // Extension method for dependency injection registration
        public static IServiceCollection AddDataSeeders(this IServiceCollection services)
        {
            services.AddScoped<EnumDataSeeder>();
            services.AddScoped<CustomDataSeeder>();

            return services;
        }
    }
}
