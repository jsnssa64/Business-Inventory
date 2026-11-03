using Domain.Interface;
using Grpc.Core;
using Infrastructure.Factory;
using Infrastructure.Repository.Inventory;
using Infrastructure.Repository.Product;
using Infrastructure.Repository.RoleRepo;
using Infrastructure.Repository.UserRepo;
using Infrastructure.Repository.Webhook;
using InventoryApi.Authentication;
using KurrentDB.Client;
using MassTransit;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Services.Service.Inventory;
using Services.Service.Product;
using Services.Service.Role;
using Services.Service.Security;
using Services.Service.Security.Models;
using Services.Service.Security.Utilities;
using Services.Service.User;
using Services.Service.Webhook;
using Shared.Constants;
using Shared.Utilities.User;
using ZiggyCreatures.Caching.Fusion;
using UserCredentials = KurrentDB.Client.UserCredentials;

namespace InventoryApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddTransient<IJWTUtility, JWTUtility>();
            services.Configure<Security>(configuration.GetSection("Security"));
            services.AddScoped<ISecurityService, SecurityService>();
            return services;
        }

        public static IServiceCollection AddWebhook(this IServiceCollection services)
        {
            services.AddScoped<IWebhookService, WebhookService>();
            services.AddScoped<IWebhookRepository, WebhookRepository>();
            return services;
        }

        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            services.AddTransient<IUserUtility, UserUtility>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            return services;
        }

        public static string GetDataConnection(this IConfigurationManager configuration, string name)
        {
            var connectionString = configuration.GetConnectionString(name);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{name}' is not configured.");
            }
            return connectionString;
        }

        public static IServiceCollection AddEventServices(this IServiceCollection services, IConfigurationManager configuration)
        {
            var rabbitmqConnectionString = configuration.GetDataConnection(DbConnectionSettings.RabbitMQ);

            services.AddMassTransit(config =>
            {
                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitmqConnectionString);
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            services.AddKurrentDBClient((KurrentDBClientSettings settings) =>
            {
                settings.ConnectionName = configuration.GetDataConnection(DbConnectionSettings.KurrentDb);
                settings.ChannelCredentials = ChannelCredentials.Insecure;

                //settings.ConnectivitySettings = new KurrentDBClientConnectivitySettings();
                //settings.CreateHttpMessageHandler = () => new HttpClientHandler
                //{
                //    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // For development purposes only, do not use in production
                //};

                settings.DefaultCredentials = new UserCredentials("Test", "Test");
                settings.DefaultDeadline = TimeSpan.FromSeconds(30);
                //settings.OperationOptions = new KurrentDBClientOperationOptions()
                //{
                //    BatchAppendSize = 100,
                //    GetAuthenticationHeaderValue = (k, v) => { },
                //    ThrowOnAppendFailure = true
                //};
            });

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
            var connectionString = configuration.GetDataConnection(DbConnectionSettings.InventoryDb) ?? throw new InvalidOperationException("Database:Address configuration is missing or invalid.");
            
            services.AddSingleton<IDbConnectionFactory, DapperDbConnectionFactory>(sp =>
            {
                var connectionDict = new Dictionary<string, string>()
                {
                        {
                            DbConnectionSettings.InventoryDb,
                            connectionString
                        }
                };

                return new DapperDbConnectionFactory(connectionDict);
            });

            return services;
        }

        public static IServiceCollection AddLoginAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(JWTCookie.JwtCookieScheme)
                .AddScheme<AuthenticationSchemeOptions, CookieJwtHandler>(JWTCookie.JwtCookieScheme, options => { });

            services.Configure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = JWTCookie.JwtCookieScheme;
                options.DefaultChallengeScheme = JWTCookie.JwtCookieScheme;
            });

            return services;
        }
    }
}
