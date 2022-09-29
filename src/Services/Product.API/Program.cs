using Common.Logging;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Product.API.Extensions;
using Product.API.Persistence;
using Serilog;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

Log.Information("Start Product API up");

try
{
    builder.Host.AddAppConfigurations();
    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
            $"{builder.Environment.ApplicationName} v1"));
    }
    app.UseInfrastructure();
    app.UseMiddleware<ErrorWrappingMiddleware>();
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapDefaultControllerRoute();
    });
    app.MigrateDatabase<ProductContext>((context, _) =>
    {
        ProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    })
        .Run();

}

catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal))
    {
        throw;
    }

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information("Shut down Product API complete");
    Log.CloseAndFlush();
}