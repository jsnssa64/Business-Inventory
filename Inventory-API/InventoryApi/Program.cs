using System.Data.Entity.Infrastructure;
using EventStore.Client;
using InventoryApi.Extensions;
using InventoryApi.Factory;
using InventoryApi.Repository;
using InventoryApi.Service.InventoryService;
using Microsoft.Extensions.DependencyInjection.Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDapper(
                builder.Configuration.GetConnectionString(DatabaseConnections.InventoryDb.ToString()));

        builder.Services.AddEventStore(
                builder.Configuration.GetSection("EventStore:Address").Get<Uri>());




        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}