using Microsoft.EntityFrameworkCore;
using StatisticService.BLL.Abstractions.Repository;
using StatisticService.BLL.Entity;

namespace StatisticService.DAL.Repository
{
    public class DefaultYearStatisticRepository : IDefaultYearStatisticRepository
    {
        private readonly ApplicationContext _context;

        public DefaultYearStatisticRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task EditDefaultYearStatistic(DefaultYearStatisticEntity entity)
        {
            _context.DefaultYearStatisticEntities.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<DefaultYearStatisticEntity?> GetDefaultYearStatistic(int year)
        {
            return await _context.DefaultYearStatisticEntities.FirstOrDefaultAsync(x => x.Year == year);
        }

        public async Task SaveDefaultYearStatistic(DefaultYearStatisticEntity entity)
        {
            _context.DefaultYearStatisticEntities.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
}
