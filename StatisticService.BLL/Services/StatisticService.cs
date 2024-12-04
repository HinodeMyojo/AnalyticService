using StatisticService.BLL.Abstractions;
using StatisticService.BLL.Dto;
using StatisticService.BLL.Dto.YearStatistic;
using StatisticService.BLL.Entity;
using StatisticService.DAL.Repository;

namespace StatisticService.BLL.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _repository;
        private const int INITIAL_ATTEMPT_COUNT = 0;
        private const int COUNT_DAYS_IN_WEEK = 7;
        private const int COUNT_MONTHS_IN_YEAR = 12;

        public StatisticService(IStatisticRepository repository)
        {
            _repository = repository;
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
        public async Task<ResponseYearStatisticDto> GetYearStatisticAsync(int userId, int year)
        {

            YearStatisticData[][] result;

            DateTime firstDay = new(year, 1, 1);
            DateTime lastDay = new(year, 12, 31);

            StatisticEntity? responseFromDB = await _repository
                .GetStatisticAsync(x => x.UserId == userId && x.AnsweredAt > firstDay && x.AnsweredAt < lastDay);

            if (responseFromDB == null)
            {
                result = GenerateEmptyYearStatistic(year);
            }

            YearStatisticData[][] jaggedArray = new YearStatisticData[2][];


            ResponseYearStatisticDto response = new()
            {
                Colspan = [1, 23, 4],
                Data = jaggedArray,
                Year = 2024
            };

            return response;
        }

        /// <summary>
        /// Метод для получения статистики за год. Сначала пытается найти в БД данные по указанному year.
        /// Если в БД данных нет - генерирует, добавляет их в БД и отдает
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static YearStatisticData[][] GetYearStatistic(int year)
        {
            YearStatisticData[][] jaggedArray = new YearStatisticData[2][];

            return jaggedArray;
        }

        /// <summary>
        /// Генерирует статистику за год
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        private static YearStatisticData[][] GenerateEmptyYearStatistic(int year)
        {
            // Инициализация объекта для хранения данных
            YearStatisticModel yearStatistic = new()
            {
                Year = year
            };

            Random random = new();

            // Проходим по каждому месяцу года
            for (int month = 1; month <= COUNT_MONTHS_IN_YEAR; month++)
            {
                // Получаем количество дней в текущем месяце
                int daysInMonth = DateTime.DaysInMonth(year, month);

                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateTime date = new(year, month, day);

                    // Добавляем данные текущего дня
                    yearStatistic.Data.Add(new YearStatisticData
                    {
                        Date = date,
                        Value = 0
                    });

                    // Для первого дня января добавляем пустые дни до первого дня недели
                    if (month == 1 && day == 1)
                    {
                        // Определяем какой день недели по счету является первое января
                        int firstDayOfWeek = (int)date.DayOfWeek;

                        // Добавляем пустые дни до первого дня (если не воскресенье)
                        for (int i = 0; i < firstDayOfWeek; i++)
                        {
                            yearStatistic.Data.Insert(0, new YearStatisticData
                            {
                                Date = date.AddDays(-1 - i), // Смещение на предыдущие дни
                                Value = null // Пустое значение для отсутствующих дней
                            });
                        }
                    }
                }
            }

            // Создаем двумерный массив для группировки по дням недели
            YearStatisticData[][] result = new YearStatisticData[COUNT_DAYS_IN_WEEK][];

            for (int i = 0; i < COUNT_DAYS_IN_WEEK; i++)
            {
                // Фильтруем данные по конкретному дню недели (0 = Воскресенье, 6 = Суббота)
                result[i] = yearStatistic.Data
                    .Where(x => x.Date.DayOfWeek == (DayOfWeek)i)
                    .ToArray();
            }

            return result;
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
