using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build(); 

app.MapHub<ChatHub>("/hub");

app.Run();
