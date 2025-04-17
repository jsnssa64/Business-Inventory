using EventStore.Client;
using InventoryApi.Factory;
using System.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace InventoryApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, IConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString(DatabaseConnections.InventoryDb.ToString());
            if (connectionString == null)
            {
                throw new InvalidOperationException("Database:Address configuration is missing or invalid.");
            }

            services.AddSingleton<IDbConnectionFactory, DapperDbConnectionFactory>(sp =>
            {
                var connectionDict = new Dictionary<DatabaseConnections, string>()
                {
                        {
                            DatabaseConnections.InventoryDb,
                            connectionString
                        }
                };

                return new DapperDbConnectionFactory(connectionDict);
            });

            return services;
        }

        public static IServiceCollection AddEventStore(this IServiceCollection services, IConfigurationManager configuration)
        {
            var eventStoreUri = configuration.GetSection("EventStore:Address").Get<Uri>();
            if (eventStoreUri == null)
            {
                throw new InvalidOperationException("EventStore:Address configuration is missing or invalid.");
            }

            services.AddSingleton(new EventStoreClient(new EventStoreClientSettings
            {
                ConnectivitySettings = new EventStoreClientConnectivitySettings
                {
                    Address = eventStoreUri
                }
            }));

            return services;
        }

        public static IServiceCollection AddLoginAuthentication(this IServiceCollection services)
        {
            //return services;
            services
                .AddAuthentication("JWTLogin");

            return services;
        }
    }
}
