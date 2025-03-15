using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Vanguard.Infrastructure;

namespace Vanguard.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /* Check for migration command */
            var migrationOnly = args.Length > 0 && args[0] == "migrate";

            /* Add services to the container */
            ConfigureServices(builder);

            var app = builder.Build();

            /* Run migrations if specifically requested */
            if (migrationOnly)
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<VanguardContext>();
                    dbContext.Database.Migrate();
                }

                Log.Information("Migration completed successfully");
                return;
            }

            /* Configure the HTTP request pipeline */
            ConfigureApp(app);

            /* Start the application */
            var port = Environment.GetEnvironmentVariable("PORT") ?? "27324";
            if (!string.IsNullOrWhiteSpace(port))
            {
                Log.Information("Using Heroku port: {Port}", port);
                app.Urls.Clear();
                app.Urls.Add($"http://*:{port}");
            }

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            builder.Host.UseSerilog();

            // Configure database
            ConfigureDatabase(builder);

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:4200",
                            "https://lhajoosten.github.io"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            // Add controllers with Newtonsoft JSON support
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            // API versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddMvc();

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Vanguard Platform API",
                    Version = "v1",
                    Description = "API for the Vanguard Skill Development Platform"
                });

                c.EnableAnnotations();

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

            builder.Services.AddSwaggerGenNewtonsoftSupport();

            // Authentication and Authorization
            ConfigureAuthentication(builder);

            // Register dependencies
            RegisterServices(builder);
        }

        private static void ConfigureDatabase(WebApplicationBuilder builder)
        {
            // Check for Heroku SQL Server connection string format
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                // Parse Heroku-style connection string for SQL Server
                try
                {
                    var uri = new Uri(databaseUrl);
                    var userInfo = uri.UserInfo.Split(':');
                    var username = userInfo[0];
                    var password = userInfo[1];
                    var host = uri.Host;
                    var database = uri.AbsolutePath.TrimStart('/');

                    var connectionString = $"Server={host};Database={database};User Id={username};Password={password};TrustServerCertificate=True;";

                    builder.Services.AddDbContext<VanguardContext>(options =>
                        options.UseSqlServer(connectionString,
                            sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

                    Log.Information("Using Heroku SQL Server connection");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error parsing DATABASE_URL");
                    throw;
                }
            }
            else
            {
                // Use connection string from configuration
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                builder.Services.AddDbContext<VanguardContext>(options =>
                    options.UseSqlServer(connectionString,
                        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

                Log.Information("Using configuration connection string");
            }
        }

        private static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            {
                // JWT Authentication
                var jwtSettings = builder.Configuration.GetSection("JwtSettings");
                var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "your-temporary-secret-key-at-least-32-chars");

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // Set to true in production
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        ClockSkew = TimeSpan.Zero
                    };
                });

                builder.Services.AddAuthorizationBuilder()
                    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
                    .AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));
            }
        }

        private static void RegisterServices(WebApplicationBuilder builder)
        {
            var services = builder.Services;

        }

        private static void ConfigureApp(WebApplication app)
        {
            // Use Serilog request logging
            app.UseSerilogRequestLogging();

            // Development-specific middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Production error handling
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // Enable Swagger in all environments for this hobby project
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Skill Development Platform API v1");
                c.RoutePrefix = string.Empty; // Set Swagger UI at the root
            });

            // Add standard middleware
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Add a basic health check endpoint
            app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
                .WithName("HealthCheck");
        }
    }
}
