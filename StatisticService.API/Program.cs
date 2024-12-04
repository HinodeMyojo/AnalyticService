using StatisticService.BLL.Abstractions.Repository;
using StatisticService.BLL.Abstractions.Service;
using StatisticService.BLL.Services;
using StatisticService.DAL;
using StatisticService.DAL.Repository;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddDbContext<ApplicationContext>();

builder.Services.AddTransient<IStatisticService, StatisticService.BLL.Services.StatisticService>();
builder.Services.AddTransient<IStatisticRepository, StatisticRepository>();

builder.Services.AddTransient<IDefaultYearStatisticRepository, DefaultYearStatisticRepository>();
builder.Services.AddTransient<IDefaultYearStatisticService, DefaultYearStatisticService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<StatisticService.API.Services.StatisticService>();

app.Run();
