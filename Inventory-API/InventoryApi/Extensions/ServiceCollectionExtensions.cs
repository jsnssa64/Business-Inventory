using EventStore.Client;
using InventoryApi.Factory;
using System.Data.Entity.Infrastructure;
using InventoryApi.Authentication;
using Microsoft.AspNetCore.Authentication;
using InventoryApi.Constants;
using InventoryApi.Repository.Inventory;
using InventoryApi.Repository.RoleRepo;
using InventoryApi.Repository;
using InventoryApi.Service.InventoryService;
using InventoryApi.Service.RoleService;
using InventoryApi.Service.UserService.Utility;
using InventoryApi.Service.UserService;
using InventoryApi.Service.SecurityService.Models;
using InventoryApi.Service.SecurityService;
using MassTransit;
using static MassTransit.Logging.OperationName;

namespace InventoryApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.Configure<Security>(configuration.GetSection("Security"));
            services.AddSingleton<ISecurityService, SecurityService>();
            return services;
        }
        
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddSingleton<IUserUtility, UserUtility>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddSingleton<IInventoryService, InventoryService>();
            services.AddSingleton<IInventoryRepository, InventoryRepository>();

            services.AddSingleton<IRoleService, RoleService>();
            services.AddSingleton<IRoleRepository, RoleRepository>();

            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IProductService, ProductService>();

            return services;
        }

        public static IServiceCollection AddMemoryServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString(DatabaseConnections.RabbitMQ.ToString());
            if (connectionString == null)
            {
                throw new InvalidOperationException("ServiceBus:Address configuration is missing or invalid.");
            }

            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(connectionString);
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            services.AddDapper(configuration);
            services.AddEventStore(configuration);
            return services;
        }

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
