using StatisticService.BLL.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddTransient<IStatisticService, StatisticService.BLL.Services.StatisticService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<StatisticService.API.Services.StatisticGrpcService>();

app.Run();
