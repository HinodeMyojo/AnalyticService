using Microsoft.EntityFrameworkCore;
using StatisticService.BLL.Abstractions.Repository;
using StatisticService.BLL.Entity;
using System.Linq.Expressions;

namespace StatisticService.DAL.Repository
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly ApplicationContext _context;

        public StatisticRepository(ApplicationContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Удаляет сущность статистики из БД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task DeleteStatisticAsync(int id)
        {
            try
            {
                StatisticEntity? entityFromDb = await GetStatisticAsync(x => x.Id == id);

                if (entityFromDb == null)
                {
                    throw new Exception("Не удалось удалить сущность статистики из базы. Такой сущности статистики не существует!");
                }
                _context.StatisticEntities.Remove(entityFromDb);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Не удалось удалить сущность статистики из базы");
            }
        }

        public async Task EditStatisticAsync(StatisticEntity entity)
        {
            try
            {
                StatisticEntity? entityFromDb = await GetStatisticAsync(x => x.Id == entity.Id);

                if (entityFromDb == null)
                {
                    throw new Exception("Не удалось изменить сущность статистики id: {entity.Id}. Такой сущности статистики не существует!");
                }

                entityFromDb.AttemptCount = entity.AttemptCount;
                entityFromDb.ModuleId = entity.ModuleId;
                entityFromDb.Id = entity.Id;
                entityFromDb.UserId = entity.UserId;
                entityFromDb.AnsweredAt = entity.AnsweredAt;
                entityFromDb.Elements = entity.Elements;

                _context.Update(entityFromDb);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception($"Не удалось изменить сущность статистики id: {entity.Id}");
            }
        }

        public async Task<StatisticEntity?> GetStatisticAsync(Expression<Func<StatisticEntity, bool>> predicate)
        {
            try
            {
                StatisticEntity? result = await _context
                    .StatisticEntities.Where(predicate).Include(x => x.Elements).FirstOrDefaultAsync();
                return result;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> SaveStatisticAsync(StatisticEntity entity)
        {
            try
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return entity.Id;
            }
            catch
            {
                throw new Exception("Не удалось сохранить сущность статистики!");
            }
        }
    }
}
