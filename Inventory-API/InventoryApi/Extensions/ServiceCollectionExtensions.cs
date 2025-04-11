using EventStore.Client;
using InventoryApi.Factory;
using System.Data.Entity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IDbConnectionFactory, DapperDbConnectionFactory>(sp =>
            {
                var connectionDict = new Dictionary<DatabaseConnections, string?>()
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

        public static IServiceCollection AddEventStore(this IServiceCollection services, Uri eventStoreUri)
        {
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
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddBearerToken("", jwtOptions =>
                {
                    jwtOptions.MetadataAddress = builder.Configuration["Api:MetadataAddress"];
                    // Optional if the MetadataAddress is specified
                    jwtOptions.Authority = builder.Configuration["Api:Authority"];
                    jwtOptions.Audience = builder.Configuration["Api:Audience"];
                    jwtOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudiences = builder.Configuration.GetSection("Api:ValidAudiences").Get<string[]>(),
                        ValidIssuers = builder.Configuration.GetSection("Api:ValidIssuers").Get<string[]>()
                    };

                    jwtOptions.MapInboundClaims = false;
                });
        }
    }
}
