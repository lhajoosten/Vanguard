using Vanguard.Application.Common.Services;
using Vanguard.WebApi.Services;

namespace Vanguard.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
