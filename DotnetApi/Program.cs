using DotnetApi;
using DotnetApi.Endpoint;
using DotnetApi.Setup;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.AddAuthentication()
    .AddAuthorization()
    .AddValidation()
    .AddOpenApi()
    .AddLogging();

var app = builder.Build();

app.MapApiEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithPreferredScheme("Bearer")
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
            .WithTheme(ScalarTheme.Kepler);
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");

if (bool.TryParse(app.Configuration["Api:AutoUpdateDb"], out var autoUpdateDb))
{
    if (autoUpdateDb)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
    }
}

app.Run();