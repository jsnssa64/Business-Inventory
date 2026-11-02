using MassTransit;
using Microsoft.Extensions.Options;
using NotificationService.Configuration;
using NotificationService.Configuration.Constants;
using NotificationService.Configuration.Enum;
using NotificationService.Configuration.ServiceBus;
using NotificationService.Consumer.Notification;
using NotificationService.Hubs;
using Shared.Constants;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSignalR();

        var connectionString = builder.Configuration.GetConnectionString(DatabaseConnections.RabbitMQ);

        builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(DatabaseConnections.RabbitMQ));


        builder.Services.AddMassTransit(transit =>
        {
            transit.AddConsumer<SMSNotificationConsumer>(configuration =>
            {
                configuration.ConcurrentMessageLimit = 1000;
                configuration.UseMessageRetry(x => x.Immediate(1));
            });

            var busOptions = builder.Configuration.GetSection(ServiceBusConstants.ServiceBus).Get<ServiceBusOptions>()
                ?? throw new InvalidOperationException("ServiceBus configuration is missing");

            transit.SetKebabCaseEndpointNameFormatter();

            switch (busOptions.Type)
            {
                case nameof(ServiceBusType.InMemory):
                    transit.UsingInMemory(
                        (context, cfg) =>
                        {
                            cfg.UseScheduledRedelivery(r =>
                                r.Intervals(
                                    TimeSpan.FromMinutes(5),
                                    TimeSpan.FromMinutes(15),
                                    TimeSpan.FromMinutes(30)
                                )
                            );
                            cfg.UseMessageRetry(r => r.Immediate(5));
                            cfg.ConfigureEndpoints(context);
                        }
                    );
                    break;
                case nameof(ServiceBusType.RabbitMQ):
                    transit.UsingRabbitMq(
                        (context, cfg) =>
                        {
                            var options = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;

                            cfg.Host(
                                options.HostName, 
                                options.VirtualHost, h =>
                                {
                                    h.Username(options.Username ?? throw new InvalidOperationException("RabbitMQ username is not configured"));
                                    h.Password(options.Password ?? throw new InvalidOperationException("RabbitMQ password is not configured"));
                                });
                            cfg.ConfigureEndpoints(context);
                            cfg.UseScheduledRedelivery(r =>
                                r.Intervals(
                                    TimeSpan.FromMinutes(5),
                                    TimeSpan.FromMinutes(15),
                                    TimeSpan.FromMinutes(30)
                                )
                            );
                            cfg.UseMessageRetry(r => r.Immediate(5));
                            cfg.ConfigureEndpoints(context);
                        }
                    );
                    break;
                default:
                    break;
            }
        });

        var app = builder.Build();

        app.MapHub<NotificationHub>("/hub");

        app.Run();
    }
}