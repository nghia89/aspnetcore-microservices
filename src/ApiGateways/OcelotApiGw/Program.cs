using Common.Logging;
using Infrastructure.Middlewares;
using Ocelot.Middleware;
using OcelotApiGw.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    // Add services to the container.
    builder.Host.AddAppConfigurations();
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureOcelot(builder.Configuration);
    builder.Services.ConfigureCors(builder.Configuration);
    var app = builder.Build();

    //Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.UseSwagger();
        //app.UseSwaggerUI(c =>
        //{
        //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
        //        $"{builder.Environment.ApplicationName} v1"));
        //});
    }


    app.UseCors("CorsPolicy");
    app.UseMiddleware<ErrorWrappingMiddleware>();

    app.UseAuthentication();
    app.UseRouting();
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", context =>
        {
            context.Response.Redirect("swagger/index.html");
            return Task.CompletedTask;
        });
    });

    app.UseSwaggerForOcelotUI(
      opt => { opt.PathToSwaggerGenerator = "/swagger/docs"; });
    app.MapControllers();
    await app.UseOcelot();
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
    Log.Information("Shut down Basket API complete");
    Log.CloseAndFlush();
}