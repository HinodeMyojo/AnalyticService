using StatisticService.BLL.Abstractions;
using StatisticService.DAL.Repository;

namespace StatisticService.BLL.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository repository;
    }
}
