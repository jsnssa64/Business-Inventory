using MassTransit;
using NotificationService.Consumer.Notifications;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var connectionString = builder.Configuration.GetConnectionString();

builder.Services.AddMassTransit(transit =>
{
    transit.AddConsumer<SMSNotificationConsumer>(configuration => {
        configuration.ConcurrentMessageLimit = 1000;
        configuration.UseMessageRetry(x => x.Immediate(1));
    });

    transit.SetKebabCaseEndpointNameFormatter();

    switch (buOptions.Type)
    {
        case ServiceBusType.InMemory:
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
        case ServiceBusType.RabbitMQ:
            transit.UsingRabbitMq(
                (context, cfg) =>
                {
                    cfg.Host(options.HostName, options.VirtualHost, h =>
                    {
                        h.Username(options.UserName);
                        h.Password(options.Password);
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
    }
}

var app = builder.Build(); 

app.MapHub<ChatHub>("/hub");

app.Run();
