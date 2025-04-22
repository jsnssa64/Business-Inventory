using EventStore.Client;
using InventoryApi.Factory;
using System.Data.Entity.Infrastructure;
using InventoryApi.Authentication;
using Microsoft.AspNetCore.Authentication;
using InventoryApi.Constants;

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
            var connectionString = configuration.GetConnectionString(DatabaseConnections.EventStoreDb.ToString());
            if (connectionString == null)
            {
                throw new InvalidOperationException("Database:Address configuration is missing or invalid.");
            }

            var eventStoreUri = new Uri(connectionString);

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
            services
                .AddAuthentication(Cookie.JwtCookieScheme)
                .AddScheme<AuthenticationSchemeOptions, CookieJwtHandler>(Cookie.JwtCookieScheme, options => { });

            services.Configure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = Cookie.JwtCookieScheme;
                options.DefaultChallengeScheme = Cookie.JwtCookieScheme;
            });

            return services;
        }
    }
}
