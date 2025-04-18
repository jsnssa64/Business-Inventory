using InventoryApi.Extensions;
using InventoryApi.Repository;
using InventoryApi.Repository.Inventory;
using InventoryApi.Repository.RoleRepo;
using InventoryApi.Service.InventoryService;
using InventoryApi.Service.RoleService;
using InventoryApi.Service.SecurityService;
using InventoryApi.Service.SecurityService.Models;
using InventoryApi.Service.UserService;

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

        builder.Services.AddDapper(builder.Configuration);
        builder.Services.AddEventStore(builder.Configuration);

        builder.Services.AddSingleton<ISecurityService, SecurityService>();
        builder.Services.AddSingleton<IJWTUtility, JWTUtility>();
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IUserRepository, UserRepository>();
        builder.Services.AddSingleton<IInventoryService, InventoryService>();
        builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();
        builder.Services.AddSingleton<IRoleService, RoleService>();
        builder.Services.AddSingleton<IRoleRepository, RoleRepository>();

        builder.Services.Configure<Security>(builder.Configuration.GetSection("Security"));

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
