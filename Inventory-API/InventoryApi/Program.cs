using InventoryApi.Extensions;
using InventoryApi.Repository.Webhook;
using InventoryApi.Service.SecurityService;
using InventoryApi.Service.UserService;
using Microsoft.Extensions.Caching.Memory;
using ZiggyCreatures.Caching.Fusion;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddMemoryServices(builder.Configuration);
        builder.Services.AddSecurityServices(builder.Configuration);

        builder.Services.AddSingleton<IJWTUtility, JWTUtility>();

        builder.Services.AddApiServices(builder.Configuration);

        builder.Services.AddSingleton<IWebhookService, WebhookService>();
        builder.Services.AddSingleton<IWebhookRepository, WebhookRepository>();

        builder.Services.AddLoginAuthentication();

        builder.Services.AddFusionCache()
            .WithDefaultEntryOptions(new FusionCacheEntryOptions
            {
                Duration = TimeSpan.FromMinutes(2),
                Priority = CacheItemPriority.Low
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseJwtCookieAuth();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
