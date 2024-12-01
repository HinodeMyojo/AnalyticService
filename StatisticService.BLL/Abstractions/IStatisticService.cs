
using StatisticService.BLL.Dto;

namespace StatisticService.BLL.Abstractions
{
    public interface IStatisticService
    {
        Task<ResponseYearStatisticDto> GetYearStatisticAsync(int userId, int year);
        Task<ResponseStatisticDto> SaveStatisticAsync(RequestStatisticDto model);
    }
}