using PropertyCare.API;
using PropertyCare.Application;
using PropertyCare.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Layered service registration (Clean Architecture)
builder.Services.AddAPI(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS for the Angular dev server (origins come from configuration)
const string corsPolicy = "AllowAngularDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:4200"])
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply migrations and seed demo data (environment-aware)
await DatabaseInitializer.InitializeDatabaseAsync(app.Services, app.Environment);

await app.RunAsync();
