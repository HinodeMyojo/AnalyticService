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

            IEnumerable<StatisticEntity> responseFromDB = await _repository
                .GetAllStatisticsAsync(x => x.UserId == userId && x.AnsweredAt.Year == year);

            YearStatisticDto defaultYearStatistic = await _defaultYearStatisticService
                .GetOrCreateDefaultYearStatistic(year);

            YearStatisticDto resultYearStatistic;

            if (responseFromDB.Count() != 0)
            {
                resultYearStatistic = SetUserStatisticToDefault(defaultYearStatistic, responseFromDB);
                return resultYearStatistic;
            }


            resultYearStatistic = defaultYearStatistic;
            return resultYearStatistic;
        }
        //private static YearStatisticDto SetUserStatisticToDefault(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB)
        //{
        //    //const int OFFSET = 1;

        //    DateTime answerDay;
        //    Parallel.ForEach(responseFromDB, element =>
        //    {
        //        answerDay = element.AnsweredAt;

        //        int dayOfWeek = (int)answerDay.DayOfWeek;

        //        YearStatisticData? selectedDate = defaultYearStatistic
        //        .Data[dayOfWeek]
        //        .Where(x => x.Date.DayOfYear == answerDay.DayOfYear)
        //        .FirstOrDefault();

        //        if (selectedDate != null)
        //        {
        //            YearStatisticData day = selectedDate;
        //            day.Value++;
        //        }
        //    });

        //    return defaultYearStatistic;
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

        // Private methods
        #region

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultYearStatistic"></param>
        /// <param name="responseFromDB"></param>
        /// <returns></returns>
        private static YearStatisticDto SetUserStatisticToDefault(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB)
        {
            foreach (var element in responseFromDB)
            {
                DateTime answerDay = element.AnsweredAt;
                int dayOfWeek = (int)answerDay.DayOfWeek;

                YearStatisticData? selectedDate = defaultYearStatistic
                    .Data[dayOfWeek]
                    .FirstOrDefault(x => x.Date.DayOfYear == answerDay.DayOfYear);

                if (selectedDate != null)
                {
                    selectedDate.Value++;
                }
            }

            return defaultYearStatistic;
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
