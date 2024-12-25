using StatisticService.BLL.Abstractions.Repository;
using StatisticService.BLL.Abstractions.Service;
using StatisticService.BLL.Services;
using StatisticService.DAL.Repository;
using StatisticService.DAL;

namespace StatisticService.API
{
    public static class ProgramService
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>();

            services.AddTransient<IStatisticService, StatisticService.BLL.Services.StatisticService>();
            services.AddTransient<IStatisticRepository, StatisticRepository>();

            services.AddTransient<IDefaultYearStatisticRepository, DefaultYearStatisticRepository>();
            services.AddTransient<IDefaultYearStatisticService, DefaultYearStatisticService>();

            return services;
        }
    }
}
