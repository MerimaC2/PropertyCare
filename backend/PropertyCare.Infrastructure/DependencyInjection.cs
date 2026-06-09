using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyCare.Application.Abstractions;
using PropertyCare.Domain.Entities.Identity;
using PropertyCare.Infrastructure.Auth;
using PropertyCare.Infrastructure.Database;
using PropertyCare.Infrastructure.Database.Seeders;

namespace PropertyCare.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // Database (connection string lives in appsettings.{Environment}.json)
        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly("PropertyCare.Infrastructure")));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<DatabaseContext>());

        // JWT settings + token service
        var jwtOptions = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT options are not configured.");
        services.AddSingleton(jwtOptions);
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Password hashing (same hasher ASP.NET Core Identity uses)
        services.AddScoped<IPasswordHasher<AppUserEntity>, PasswordHasher<AppUserEntity>>();

        // Demo data seeding
        services.AddScoped<IDatabaseSeeder, DynamicDataSeeder>();

        return services;
    }
}
