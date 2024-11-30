
namespace StatisticService.BLL.Abstractions
{
    public interface IStatisticService
    {
        Task GetYearStatisticAsync(int userId, int year);
        Task SaveStatisticAsync(int moduleId, int userId, object elements, DateTime completedAt);
    }
}