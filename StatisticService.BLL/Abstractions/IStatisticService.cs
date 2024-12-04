
using StatisticService.BLL.Dto;
using StatisticService.BLL.Dto.YearStatistic;

namespace StatisticService.BLL.Abstractions
{
    public interface IStatisticService
    {
        Task<ResponseStatisticDto> GetStatisticById(int id);
        Task<ResponseYearStatisticDto> GetYearStatisticAsync(int userId, int year);
        Task<int> SaveStatisticAsync(RequestStatisticDto model);
    }
}