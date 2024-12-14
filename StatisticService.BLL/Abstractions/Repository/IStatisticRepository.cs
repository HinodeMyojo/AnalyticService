using StatisticService.BLL.Entity;
using System.Linq.Expressions;

namespace StatisticService.BLL.Abstractions.Repository
{
    public interface IStatisticRepository
    {
        Task<StatisticEntity?> GetStatisticAsync(Expression<Func<StatisticEntity, bool>> entity);
        Task<int> SaveStatisticAsync(StatisticEntity entity);
        Task EditStatisticAsync(StatisticEntity entity);
        Task DeleteStatisticAsync(int id);
        Task<IEnumerable<StatisticEntity>> GetAllStatisticsAsync(Expression<Func<StatisticEntity, bool>> predicate);
        Task<List<StatisticEntity>> GetLastActivity(int userId);
    }
}
