using InventoryApi.Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApiServices(builder.Configuration);

        builder.Services.AddMemoryServices(builder.Configuration);
        builder.Services.AddSecurityServices(builder.Configuration);
        builder.Services.AddDatabaseServices(builder.Configuration);
        builder.Services.AddEventServices(builder.Configuration);

        builder.Services.AddWebhook();

        builder.Services.AddLoginAuthentication();

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
