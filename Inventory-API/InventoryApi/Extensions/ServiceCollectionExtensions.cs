using EventStore.Client;
using InventoryApi.Factory;
using System.Data.Entity.Infrastructure;
using InventoryApi.Authentication;
using Microsoft.AspNetCore.Authentication;
using InventoryApi.Constants;
using Services.Repository.Inventory;
using Services.Repository.RoleRepo;
using Services.Service.InventoryService;
using Services.Service.RoleService;
using Services.Service.UserService.Utility;
using Services.Service.UserService;
using Services.Service.SecurityService.Models;
using Services.Service.SecurityService;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using ZiggyCreatures.Caching.Fusion;
using Services.Repository.Webhook;
using KurrentDB.Client;
using Services.Repository.UserRepo;
using Services.Service.ProductService;

namespace InventoryApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfigurationManager configuration)
        {

            services.AddSingleton<IJWTUtility, JWTUtility>();
            services.Configure<Security>(configuration.GetSection("Security"));
            services.AddSingleton<ISecurityService, SecurityService>();
            return services;
        }

        public static IServiceCollection AddWebhook(this IServiceCollection services)
        {
            services.AddSingleton<IWebhookService, WebhookService>();
            services.AddSingleton<IWebhookRepository, WebhookRepository>();
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

            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IProductRepository, ProductRepository>();

            return services;
        }

        public static IServiceCollection AddEventServices(this IServiceCollection services, IConfigurationManager configuration)
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
            services.AddEventStore(configuration);

            return services;
        }

        public static IServiceCollection AddMemoryServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddFusionCache()
                .WithDefaultEntryOptions(new FusionCacheEntryOptions
                {
                    Duration = TimeSpan.FromMinutes(2),
                    Priority = CacheItemPriority.Low
                });

            return services;
        }

        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddDapper(configuration);
            return services;
        }

        private static IServiceCollection AddDapper(this IServiceCollection services, IConfigurationManager configuration)
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

        private static IServiceCollection AddEventStore(this IServiceCollection services, IConfigurationManager configuration)
        {
            var connectionString = configuration.GetConnectionString(DatabaseConnections.KurrentDb.ToString());
            if (connectionString == null)
            {
                throw new InvalidOperationException("Database:Address configuration is missing or invalid.");
            }

            var settings = KurrentDBClientSettings.Create(connectionString);

            services.AddSingleton(new KurrentDBClient(settings));

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
