﻿using Contracts.Common.Interfaces;
using Contracts.Domains.Interfaces;
using Contracts.Identity;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;
using Shared.Configurations;
using System.Text;

namespace Product.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection(nameof(JwtSettings))
                .Get<JwtSettings>();
            services.AddSingleton(jwtSettings);

            var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
               .Get<DatabaseSettings>();
            services.AddSingleton(databaseSettings);

            return services;
        }
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.ConfigureProductDbContext(configuration);
            services.AddInfrastructureServices();
            services.ConfigureHealthChecks();
            services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));

            return services;
        }

        private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
            var builder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);

            services.AddDbContext<ProductContext>(m => m.UseMySql(builder.ConnectionString,
                ServerVersion.AutoDetect(builder.ConnectionString), e =>
                {
                    e.MigrationsAssembly("Product.API");
                    e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                }));

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBaseAsync<,,>), typeof(RepositoryQueryBase<,,>))
                    .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                    .AddScoped<IProductRepository, ProductRepository>()
                ;
        }

        private static void ConfigureHealthChecks(this IServiceCollection services)
        {
            var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
            services.AddHealthChecks()
                .AddMySql(databaseSettings.ConnectionString, "MySql Health", HealthStatus.Degraded);
        }
    }
}
