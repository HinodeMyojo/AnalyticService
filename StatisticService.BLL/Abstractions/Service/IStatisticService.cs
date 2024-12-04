using StatisticService.BLL.Dto;
using StatisticService.BLL.Dto.YearStatistic;

namespace StatisticService.BLL.Abstractions.Service
{
    public interface IStatisticService
    {
        Task<ResponseStatisticDto> GetStatisticById(int id);
        Task<YearStatisticDto> GetYearStatisticAsync(int userId, int year);
        Task<int> SaveStatisticAsync(RequestStatisticDto model);
    }
}