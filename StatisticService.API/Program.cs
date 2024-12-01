using StatisticService.BLL.Abstractions;
using StatisticService.DAL;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddDbContext<ApplicationContext>();

builder.Services.AddTransient<IStatisticService, StatisticService.BLL.Services.StatisticService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<StatisticService.API.Services.StatisticService>();

app.Run();
