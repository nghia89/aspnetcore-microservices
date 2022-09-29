using Common.Logging;
using Contracts.Common.Interfaces;
using HealthChecks.UI.Client;
using Infrastructure.Common;
using Infrastructure.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ordering.API.Extensions;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();


builder.Host.UseSerilog(Serilogger.Configure);


try
{

    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Host.AddAppConfigurations();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApplicationServices();
    builder.Services.ConfigureMassTransit();


    builder.Services.AddControllers();
    builder.Services.AddScoped<ISerializeService, SerializeService>();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureHealthChecks();
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var orderContextSeed = scope.ServiceProvider.GetRequiredService<OrderContextSeed>();
        await orderContextSeed.InitialiseAsync();
        await orderContextSeed.SeedAsync();
    }

    app.UseMiddleware<ErrorWrappingMiddleware>();
    //app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthorization();
    app.MapControllers();
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
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information("Shut down Ordering API complete");
    Log.CloseAndFlush();
}