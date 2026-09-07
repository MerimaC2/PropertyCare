using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PropertyCare.API.Middleware;
using PropertyCare.API.Models;
using PropertyCare.API.Services;
using PropertyCare.Application.Abstractions;
using PropertyCare.Infrastructure.Auth;

namespace PropertyCare.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration config)
    {
        // Controllers + uniform validation error response for model binding failures
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                // Serialize enums (e.g. notification Type) as their string names.
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .SelectMany(e => e.Value!.Errors
                            .Select(err => new FieldErrorDto { Field = e.Key, Message = err.ErrorMessage }))
                        .ToList();

                    return new BadRequestObjectResult(new ErrorDto
                    {
                        Code = "validation.error",
                        Message = "Validation failed.",
                        Errors = errors
                    });
                };
            });

        // JWT bearer authentication
        var jwtOptions = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JWT options are not configured.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        // Every endpoint requires an authenticated user unless marked [AllowAnonymous]
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        // Current user read from JWT claims
        services.AddHttpContextAccessor();
        services.AddScoped<IAppCurrentUser, AppCurrentUser>();

        // Private file storage for request image attachments (outside wwwroot, served by a controller)
        services.AddSingleton<IFileStorageService, LocalFileStorageService>();

        // Swagger with bearer token support
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "PropertyCare API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Paste the access token from /api/auth/login."
            });
            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                { new OpenApiSecuritySchemeReference("Bearer", doc), new List<string>() }
            });
        });

        // Uniform exception responses
        services.AddExceptionHandler<AppExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
