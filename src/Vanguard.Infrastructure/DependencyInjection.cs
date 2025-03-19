using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vanguard.Common.Abstractions;
using Vanguard.Common.Base;
using Vanguard.Common.Persistence;
using Vanguard.Domain.Entities.CourseAggregate;
using Vanguard.Domain.Entities.EnrollmentAggregate;
using Vanguard.Domain.Entities.SkillAggregate;
using Vanguard.Domain.Entities.UserAggregate;
using Vanguard.Infrastructure.Persistence.Context;
using Vanguard.Infrastructure.Persistence.Repositories;
using Vanguard.Infrastructure.Persistence.Services;

namespace Vanguard.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<VanguardContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(VanguardContext).Assembly.FullName);
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            // Register repositories
            RegisterRepositories(services);

            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DomainEventService).Assembly));

            // Register domain event service
            services.AddScoped<IDomainEventService, DomainEventService>();

            // Register unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register data seeders
            services.AddDataSeeders();

            return services;
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            // Register the generic repository
            services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));

            // Register specific repositories for aggregates
            services.AddScoped<IRepository<User, UserId>, EfRepository<User, UserId>>();
            services.AddScoped<IRepository<Course, CourseId>, EfRepository<Course, CourseId>>();
            services.AddScoped<IRepository<Enrollment, EnrollmentId>, EfRepository<Enrollment, EnrollmentId>>();
            services.AddScoped<IRepository<EnrollmentCertificate, EnrollmentCertificateId>, EfRepository<EnrollmentCertificate, EnrollmentCertificateId>>();
            services.AddScoped<IRepository<Skill, SkillId>, EfRepository<Skill, SkillId>>();
            services.AddScoped<IRepository<SkillAssessment, SkillAssessmentId>, EfRepository<SkillAssessment, SkillAssessmentId>>();
        }
    }
}
