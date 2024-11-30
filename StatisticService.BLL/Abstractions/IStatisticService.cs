
using StatisticService.BLL.Dto;

namespace StatisticService.BLL.Abstractions
{
    public interface IStatisticService
    {
        Task<YearStatisticDto> GetYearStatisticAsync(int userId, int year);
        Task<ResponseStatisticModel> SaveStatisticAsync(int moduleId, int userId, object elements, DateTime completedAt);
    }
}