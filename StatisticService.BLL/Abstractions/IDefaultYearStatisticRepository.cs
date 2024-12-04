using StatisticService.BLL.Entity;

namespace StatisticService.BLL.Abstractions
{
    public interface IDefaultYearStatisticRepository
    {
        Task SaveDefaultYearStatistic(DefaultYearStatisticEntity entity);
        Task<DefaultYearStatisticEntity?> GetDefaultYearStatistic(int year);
        Task EditDefaultYearStatistic(DefaultYearStatisticEntity entity);
    }
}