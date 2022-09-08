using Contracts.ScheduledJobs;
using Infrastructure.ScheduledJobs;
using Shared.Configurations;

namespace Hangfire.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
       IConfiguration configuration)
        {
            var hangFireSettings = configuration.GetSection(nameof(HangFireSettings))
                .Get<HangFireSettings>();
            services.AddSingleton(hangFireSettings);

            return services;
        }

        internal static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, IConfiguration configuration)
        {
            var configDashboard = configuration.GetSection("HangFireSettings:Dashboard").Get<DashboardOptions>();
            var hangFireSettings = configuration.GetSection("HangFireSettings").Get<HangFireSettings>();
            var hangFireRoute = hangFireSettings.Route;

            app.UseHangfireDashboard(hangFireRoute, new DashboardOptions
            {
                // Authorization = new[] {new HangfireAuthorizationFilter()},
                DashboardTitle = configDashboard.DashboardTitle,
                StatsPollingInterval = configDashboard.StatsPollingInterval,
                AppPath = configDashboard.AppPath,
                IgnoreAntiforgeryToken = true
            });

            return app;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
       => services.AddTransient<IScheduledJobService, HangfireService>()
            
       ;
    }
}
