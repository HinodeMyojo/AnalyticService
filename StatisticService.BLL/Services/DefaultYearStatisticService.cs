using StatisticService.BLL.Abstractions.Repository;
using StatisticService.BLL.Abstractions.Service;
using StatisticService.BLL.Dto.YearStatistic;
using StatisticService.BLL.Entity;
using System.Text.Json;

namespace StatisticService.BLL.Services
{
    public class DefaultYearStatisticService : IDefaultYearStatisticService
    {
        private readonly IDefaultYearStatisticRepository _repository;

        private const int COUNT_DAYS_IN_WEEK = 7;
        private const int COUNT_MONTHS_IN_YEAR = 12;

        public DefaultYearStatisticService(IDefaultYearStatisticRepository repository)
        {
            _repository = repository;
        }

        
        public Task EditDefaultYearStatistic(YearStatisticDto model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Позволяет получить дефолтную статистику за год из базы. Если в базе ее нет — создает новую.
        /// </summary>
        /// <param name="year">Год, для которого нужна статистика.</param>
        /// <returns>Объект YearStatisticDto с данными статистики.</returns>
        /// <exception cref="InvalidOperationException">Если генерация или сохранение статистики завершилось с ошибкой.</exception>
        public async Task<YearStatisticDto> GetOrCreateDefaultYearStatistic(int year)
        {
            // Проверяем, есть ли статистика в базе
            YearStatisticDto? responseFromDb = await GetDefaultYearStatistic(year);
            if (responseFromDb != null)
            {
                return responseFromDb;
            }

            // Генерируем дефолтную статистику
            YearStatisticData[][] generatedDefaultStatistic = GenerateDefaultYearStatistic(year);

            // Извлекаем эталонную неделю
            YearStatisticData[] referenceWeek = generatedDefaultStatistic[6];

            // Генерируем Colspan для статистики
            List<int> generatedColspan = GenerateColspan(referenceWeek, year);

            // Создаём объект YearStatisticDto
            YearStatisticDto newStatistic = new()
            {
                Colspan = generatedColspan,
                Data = generatedDefaultStatistic,
                Year = year
            };

            try
            {
                // Сохраняем статистику в базе
                await SaveDefaultYearStatistic(newStatistic);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка при сохранении статистики.", ex);
            }

            return newStatistic;
        }



        public async Task<YearStatisticDto?> GetDefaultYearStatistic(int year)
        {
            DefaultYearStatisticEntity? responseFromDb = await _repository.GetDefaultYearStatistic(year);
            if (responseFromDb == null)
            {
                return null;
            }

            YearStatisticData[][]? data;
            try
            {
                // Десериализуем данные статистики
                data = JsonSerializer.Deserialize<YearStatisticData[][]>(responseFromDb.YearStatistic);
                if (data == null)
                {
                    throw new InvalidOperationException("Данные статистики не удалось десериализовать.");
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Ошибка десериализации статистики за {year} год.", ex);
            }

            // Формируем и возвращаем результат
            return new YearStatisticDto
            {
                Year = responseFromDb.Year,
                Colspan = responseFromDb.Colspan,
                Data = data
            };
        }

        public async Task SaveDefaultYearStatistic(YearStatisticDto model)
        {
            try
            {
                string yearStatistic = JsonSerializer.Serialize(model.Data); 

                DefaultYearStatisticEntity requestToDb = new()
                {
                    Colspan = model.Colspan,
                    Year = model.Year,
                    YearStatistic = yearStatistic
                };

                await _repository.SaveDefaultYearStatistic(requestToDb);

            }
            catch(JsonException ex)
            {
                throw new InvalidOperationException($"Не удалось сохранить статистику за {model.Year} год.", ex);
            }
        }

        /// <summary>
        /// Генерирует статистику за год
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static YearStatisticData[][] GenerateDefaultYearStatistic(int year)
        {
            // Инициализация объекта для хранения данных
            YearStatisticTempModel yearStatistic = new()
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
                    // Формируем дату в формате UTC
                    DateTime date = new(year, month, day, 0, 0, 0, DateTimeKind.Utc);

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
        /// Генерирует Colspan
        /// </summary>
        /// <param name="yearStatisticDatas"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private List<int> GenerateColspan(YearStatisticData[] yearStatisticDatas, int year)
        {
            List<int> result = [];
            int count = 0;
            int initialMonth = 1;
            foreach (YearStatisticData item in yearStatisticDatas)
            {
                if (item.Date.Year != year)
                {
                    continue;
                }

                if (item.Date.Month == initialMonth)
                {
                    count++;
                    continue;
                }
                result.Add(count);
                initialMonth++;
                count = 1;
            }

            return result;
        }
    }
}
