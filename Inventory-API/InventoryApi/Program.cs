using EventStore;
using EventStore.Client;
using InventoryApi.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new EventStoreClient(new EventStoreClientSettings
{
    ConnectivitySettings = new EventStoreClientConnectivitySettings
    {
        Address = builder.Configuration.GetSection("EventStore:Address").Get<Uri>()
    }
}));

builder.Services.AddSingleton<IInventoryRepository, InventoryRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
