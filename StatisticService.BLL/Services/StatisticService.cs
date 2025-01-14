using StatisticService.BLL.Abstractions.Repository;
using StatisticService.BLL.Abstractions.Service;
using StatisticService.BLL.Dto;
using StatisticService.BLL.Dto.YearStatistic;
using StatisticService.BLL.Entity;
using StatisticService.BLL.Handlers;

namespace StatisticService.BLL.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IDefaultYearStatisticService _defaultYearStatisticService;
        private readonly IStatisticHandler _statisticHandler;    
        private readonly IStatisticRepository _repository;

        private const int INITIAL_ATTEMPT_COUNT = 0;

        public StatisticService(
            IStatisticRepository repository, 
            IDefaultYearStatisticService defaultYearStatisticService, 
            IStatisticHandler statisticHandler)
        {
            _repository = repository;
            _defaultYearStatisticService = defaultYearStatisticService;
            _statisticHandler = statisticHandler;
        }

        /// <summary>
        /// Получение статистики по Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ResponseStatisticDto> GetStatisticById(int id)
        {
            StatisticEntity? statisticObject = await _repository.GetStatisticAsync(x => x.Id == id);
            if (statisticObject == null) 
            {
                string message = $"Объект статистики Id: {id} не найден!";
                throw new Exception(message);
            }

            try
            {
                CountPersentSuccess(statisticObject.Elements, out int percentSuccess);

                ResponseStatisticDto model = new()
                {
                    NumberOfAttempts = statisticObject.AttemptCount,
                    PercentSuccess = percentSuccess,
                    CompletedAt = statisticObject.AnsweredAt
                };

                return model;
            }
            catch (Exception ex)
            {
                throw new Exception("Не удалось получить информацию о процентном успехе пользователя!", ex);
            }
        }

        public async Task<List<StatisticEntity>> GetLastActivity(int userId)
        {
            return await _repository.GetLastActivity(userId);
        }

        /// <summary>
        /// Получение статистики пользователя по указанному году
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<YearStatisticDto> GetYearStatisticAsync(int userId, int year)
        {

            IEnumerable<StatisticEntity> responseFromDb = await _repository
                .GetAllStatisticsAsync(x => x.UserId == userId && x.AnsweredAt.Year == year);

            YearStatisticDto defaultYearStatistic = await _defaultYearStatisticService
                .GetOrCreateDefaultYearStatistic(year);

            YearStatisticDto resultYearStatistic;
            
            // Если в базе мы что-то нашли
            if (responseFromDb.Count() != 0)
            {
                _statisticHandler.DataHandler(defaultYearStatistic, responseFromDb);
                _statisticHandler.NumberOfActionsHandler(defaultYearStatistic, responseFromDb);
                _statisticHandler.ActiveDaysHander(defaultYearStatistic, responseFromDb);
                _statisticHandler.MaximumSeriesHander(defaultYearStatistic, responseFromDb);
                return defaultYearStatistic;
            }
            
            return defaultYearStatistic;
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

        // Private methods
        #region

        /// <summary>
        /// Вспомогательный метод по подсчету процента правильных ответов
        /// </summary>
        /// <param name="elements"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void CountPersentSuccess(List<ElementStatisticEntity> elements, out int percentSuccess)
        {
            if (elements.Count == 0)
            {
                percentSuccess = 0;
                return;
            }
            int correctAnswers = elements.Count(e => e.Answer);
            percentSuccess = (correctAnswers * 100) / elements.Count;
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
                    return INITIAL_ATTEMPT_COUNT;
                }
                return entity.AttemptCount;
            }
            catch (Exception ex)
            {
                // TODO logging
                string message = "Возникла непридвиденная ошибка при " +
                    "определении количества попыток для модуля";
                throw new Exception(message + ex);
            }
        }
        #endregion
    }
}
