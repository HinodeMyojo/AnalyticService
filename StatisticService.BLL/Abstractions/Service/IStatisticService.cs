using StatisticService.BLL.Dto;
using StatisticService.BLL.Dto.YearStatistic;
using StatisticService.BLL.Entity;

namespace StatisticService.BLL.Abstractions.Service
{
    public interface IStatisticService
    {
        Task<List<StatisticEntity>> GetLastActivity(int userId);
        Task<ResponseStatisticDto> GetStatisticById(int id);
        Task<YearStatisticDto> GetYearStatisticAsync(int userId, int year);
        Task<int> SaveStatisticAsync(RequestStatisticDto model);
    }
}