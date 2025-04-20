using Asp.Versioning;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Vanguard.Persistence.Data;
using Vanguard.WebApi.Filters;

namespace Vanguard.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

            // Global exception filter
            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });

            // API versioning
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddMvc();

            // Swagger/OpenAPI
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vanguard Platform API",
                    Version = "v1",
                    Description = "API for the Vanguard Skill Development Platform"
                });

                // Add JWT authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();

            services.AddOpenApi();

            services.AddDbContext<VanguardDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add health checks service
            services.AddHealthChecks()
                .AddCheck("API", () => HealthCheckResult.Healthy("API is running."), ["api"])
                .AddDbContextCheck<VanguardDbContext>("Database", failureStatus: HealthStatus.Degraded, tags: ["database"]);

            services.AddCors(options =>
            {
                options.AddPolicy("VanguardProductionPolicy", policy =>
                {
                    policy.WithOrigins(
                        "https://ambitious-water-04973bb03.6.azurestaticapps.net",  // Teacher portal
                        "https://gentle-tree-02d28b503.6.azurestaticapps.net"       // student portal
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });

                options.AddPolicy("VanguardDevelopmentPolicy", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:4644",  // Teacher portal
                        "http://localhost:4444"  // student portal
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            app.UseCors("VanguardProductionPolicy");
            app.UseCors("VanguardDevelopmentPolicy");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            // Health check endpoints
            RegisterHealtCheckEndpoints(app);

            app.MapControllers();

            app.Run();
        }

        private static void RegisterHealtCheckEndpoints(WebApplication app)
        {
            app.MapHealthChecks("/health/api", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("api"),
                ResponseWriter = WriteHealthCheckResponse
            });
            app.MapHealthChecks("/health/database", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("database"),
                ResponseWriter = WriteHealthCheckResponse
            });
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = WriteHealthCheckResponse
            });
        }

        private static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                Status = report.Status.ToString(),
                Components = report.Entries.Select(e => new
                {
                    Component = e.Key,
                    Status = e.Value.Status.ToString(),
                    e.Value.Description
                }),
                Duration = report.TotalDuration
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
