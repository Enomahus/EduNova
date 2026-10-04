using Infrastructure.Persistence.Entities;
using Infrastructure.Persistence.MariaDb.Contexts;
using Infrastructure.Persistence.MariaDb.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Internal;

namespace Infrastructure.Persistence.MariaDb;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureMariaDbServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Configuration de la chaîne de connexion MariaDB
        var connectionString = configuration.GetConnectionString("MariaDb");

        if (!string.IsNullOrEmpty(connectionString))
        {
            // Ajout du DbContext pour l'application
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions =>
                    {
                        mySqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        mySqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null
                        );
                    }
                );
            });

            // 2. Configuration d'Identity liée au DbContext d'écriture
            services
                .AddIdentity<UserDao, RoleDao>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 8;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Ajout du DbContext en lecture seule
            services.AddDbContext<ReadOnlyDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure();
                        mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }
                );
                // Désactive le Change Tracking globalement pour les lectures
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddDbContext<WritableDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure();
                        mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    }
                );
            });
        }

        return services;
    }

    public static IServiceCollection AddInfrastructureIdentityServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDataProtection();

        services.Configure<DataProtectionTokenProviderOptions>(
            configuration.GetSection("DataProtectionTokenProviderOptions")
        );

        services
            .AddIdentityCore<UserDao>(options =>
            {
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;

                options.Tokens.PasswordResetTokenProvider = "DataProtectorTokenProvider";
            })
            .AddTokenProvider<DataProtectorTokenProvider<UserDao>>("DataProtectorTokenProvider")
            .AddRoles<RoleDao>()
            .AddEntityFrameworkStores<WritableDbContext>();

        return services;
    }

    public static async Task UserInfrastrucutreMariaDbServicesAsync(
        this IServiceProvider serviceProvider,
        string environment
    )
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        if (environment == "NSwag")
            return;

        using var context = services.GetRequiredService<ApplicationDbContext>();
        try
        {
            await context.Database.MigrateAsync();

            var seeder = ActivatorUtilities.CreateInstance<DataSeeder>(services);
            await seeder.SeedDataAsync();

            var testSeeder = ActivatorUtilities.CreateInstance<TestDataSeeder>(services);
            await testSeeder.SeedDataAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating database.");
        }
    }
}
