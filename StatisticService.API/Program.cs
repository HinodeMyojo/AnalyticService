using Microsoft.EntityFrameworkCore;
using StatisticService.API;
using StatisticService.API.Interceptors;
using StatisticService.DAL;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddGrpc( options =>
{
    options.Interceptors.Add<ExceptionInterceptor>();
});

// ��� ����������� ��������
builder.Services.RegisterService();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<StatisticService.API.Services.StatisticService>();

using (var scope = app.Services.CreateScope())
{
    string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
    try
    {
        var dbContext =
        scope.ServiceProvider
            .GetRequiredService<ApplicationContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        throw new Exception($"I can't connect((. {connectionString}. {ex}");
    }

}

app.Run();
