using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EAVStore.DataAccess
{
    public static class DependencyConfigurator
    {
        public static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration configuration) {
            var config = configuration.GetSection(nameof(PostgreSqlConfig)).Get<PostgreSqlConfig>() ??
                         throw new InvalidOperationException(
                             $"Configuration for type {nameof(PostgreSqlConfig)} is missing or incomplete"
                         );

            services.AddSingleton(config);

            services.AddDbContext<EavStoreDbContext>(
                builder =>
                    builder.UseNpgsql(config.ToConnectionString())
            );

            return services;
        }
    }
}