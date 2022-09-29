using Hangfire.API.Extensions;
using HealthChecks.UI.Client;
using Infrastructure.ScheduledJobs;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Information("Start Product API up");

try
{
    // Add services to the container.

    builder.Services.AddControllers();
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddTeduHangfireService();
    builder.Services.ConfigureServices();
    builder.Services.ConfigureHealthChecks();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
            $"{builder.Environment.ApplicationName} v1"));
    }

    app.UseRouting();
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.UseHangfireDashboard(builder.Configuration);

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapDefaultControllerRoute();
    });

    app.Run();

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