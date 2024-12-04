using StatisticService.BLL.Dto.YearStatistic;

namespace StatisticService.BLL.Abstractions.Service
{
    public interface IDefaultYearStatisticService
    {
        Task SaveDefaultYearStatistic(YearStatisticDto model);
        Task<YearStatisticDto?> GetDefaultYearStatistic(int year);
        Task<YearStatisticDto> GetOrCreateDefaultYearStatistic(int year);
        Task EditDefaultYearStatistic(YearStatisticDto model);
    }
}