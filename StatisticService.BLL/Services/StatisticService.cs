using StatisticService.BLL.Abstractions;
using StatisticService.BLL.Dto;
using StatisticService.BLL.Entity;
using StatisticService.DAL.Repository;

namespace StatisticService.BLL.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _repository;
        private readonly int DEFAULT_ATTEMT_VALUE = 0;

        public StatisticService(IStatisticRepository repository)
        {
            _repository = repository;
        }

        public async Task<ResponseStatisticDto> GetStatisticById(int id)
        {
            StatisticEntity? statisticObject = await _repository.GetStatisticAsync(x => x.Id == id);
            if (statisticObject == null) 
            {
                string message = $"Объект статистики Id: {id} не найден!";
                throw new Exception(message);
            }


        }

        public async Task<ResponseYearStatisticDto> GetYearStatisticAsync(int userId, int year)
        {
            ResponseYearStatisticData[][] jaggedArray = new ResponseYearStatisticData[2][];


            ResponseYearStatisticDto response = new()
            {
                Colspan = [1,23,4],
                Data = jaggedArray,
                Year = 2024
            };

            return response;
        }

        /// <summary>
        /// Сервис для сохранения статистики по модулю.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> SaveStatisticAsync(RequestStatisticDto model)
        {

            int attemptCount = await FindAttemptCountModuleByUser(model.ModuleId, model.UserId);

            StatisticEntity entity = new()
            {
                AnsweredAt = model.CompletedAt,
                AttemptCount = attemptCount,
                Elements = model.Elements.Select(x => new ElementStatisticEntity()
                {
                    Answer = x.Answer,
                    ElementId = x.ElementId,
                }).ToList(),
                ModuleId = model.ModuleId,
                UserId = model.UserId,
            };

            int answer = await _repository.SaveStatisticAsync(entity);

            return answer; 
        }

        /// <summary>
        /// Вспомогательный метод для нахождения количества попыток, затраченных
        /// на модуль данным пользователем.
        /// </summary>
        /// <param name="moduleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<int> FindAttemptCountModuleByUser(int moduleId, int userId)
        {
            try
            {
                StatisticEntity? entity = await _repository
                    .GetStatisticAsync(
                    x => x.UserId == userId && x.ModuleId == moduleId);
                if (entity == null)
                {
                    return DEFAULT_ATTEMT_VALUE;
                }
                return entity.AttemptCount;
            }
            catch 
            {
                // TODO logging
                string message = "Возникла непридвиденная ошибка при " +
                    "определении количества попыток для модуля";
                throw new Exception(message);
            }
        }
    }
}
