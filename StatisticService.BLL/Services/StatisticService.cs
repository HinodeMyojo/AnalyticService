using StatisticService.BLL.Abstractions.Repository;
using StatisticService.BLL.Abstractions.Service;
using StatisticService.BLL.Dto;
using StatisticService.BLL.Dto.YearStatistic;
using StatisticService.BLL.Entity;

namespace StatisticService.BLL.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IDefaultYearStatisticService _defaultYearStatisticService;
        private readonly IStatisticRepository _repository;

        private const int INITIAL_ATTEMPT_COUNT = 0;

        public StatisticService(
            IStatisticRepository repository, 
            IDefaultYearStatisticService defaultYearStatisticService)
        {
            _repository = repository;
            _defaultYearStatisticService = defaultYearStatisticService;
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

        /// <summary>
        /// Получение статистики пользователя по указанному году
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<YearStatisticDto> GetYearStatisticAsync(int userId, int year)
        {

            YearStatisticData[][] result;

            DateTime firstDay = new(year, 1, 1);
            DateTime lastDay = new(year, 12, 31);

            StatisticEntity? responseFromDB = await _repository
                .GetStatisticAsync(x => x.UserId == userId && x.AnsweredAt > firstDay && x.AnsweredAt < lastDay);

            // Если данных об активности нет
            if (responseFromDB == null)
            {
                result = GenerateEmptyYearStatistic(year);
            }

            YearStatisticData[][] jaggedArray = new YearStatisticData[2][];


            YearStatisticDto response = new()
            {
                Colspan = [1, 23, 4],
                Data = jaggedArray,
                Year = 2024
            };

            return response;
        }

        // Private methods
        #region

        ///// <summary>
        ///// Метод для получения статистики за год. Сначала пытается найти в БД данные по указанному year.
        ///// Если в БД данных нет - генерирует, добавляет их в БД и отдает
        ///// </summary>
        ///// <param name="year"></param>
        ///// <returns></returns>
        //private async YearStatisticData[][] GetYearStatistic(int year)
        //{
        //    YearStatisticData[][] yearStatistic;

        //    try
        //    {
        //        YearStatisticDto? responseFromDB = await _defaultYearStatisticService.GetYearStatistic(year);
        //        if (responseFromDB != null)
        //        {
                    
        //        }
        //    }

        //    return jaggedArray;
        //}

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
