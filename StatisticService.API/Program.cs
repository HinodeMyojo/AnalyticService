using StatisticService.API;
using StatisticService.API.Interceptors;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddGrpc( options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
});

// Для регистрации сервисов
builder.Services.RegisterService();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<StatisticService.API.Services.StatisticService>();

app.Run();
