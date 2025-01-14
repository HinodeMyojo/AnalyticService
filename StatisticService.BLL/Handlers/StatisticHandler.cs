using StatisticService.BLL.Dto.YearStatistic;
using StatisticService.BLL.Entity;

namespace StatisticService.BLL.Handlers;

public class StatisticHandler : IStatisticHandler 
{

    /// <summary>
    /// Всопомогательный метод для добавления к дефолтной модели годовой статистики - данных пользователя
    /// </summary>
    /// <param name="defaultYearStatistic"></param>
    /// <param name="responseFromDB"></param>
    /// <returns></returns>
    public void DataHandler(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB)
    {
        foreach (StatisticEntity element in responseFromDB)
        {
            DateTime answerDay = element.AnsweredAt;
            int dayOfWeek = (int)answerDay.DayOfWeek;

            // Получаем ссылку на объект внутри defaultYearStatistic
            YearStatisticData? selectedDate = defaultYearStatistic
                .Data[dayOfWeek]
                .FirstOrDefault(x => x.Date.DayOfYear == answerDay.DayOfYear);

            if (selectedDate != null)
            {
                selectedDate.Value++;
            }
        }
    }

    /// <summary>
    /// Вычисляет кол-во активных дней пользователя
    /// </summary>
    /// <param name="defaultYearStatistic"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void ActiveDaysHander(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB)
    {
        int countActiveDays = responseFromDB.Select(x => x.AnsweredAt.DayOfYear).Distinct().Count();
        defaultYearStatistic.ActiveDays = countActiveDays;  
    }

    /// <summary>
    /// Вычисляет общеее кол-во действий пользователя
    /// </summary>
    /// <param name="defaultYearStatistic"></param>
    /// <param name="responseFromDB"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void NumberOfActionsHandler(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB)
    {
        defaultYearStatistic.NumberOfActions = responseFromDB.Count();
    }

    /// <summary>
    /// Вычисляет максимальное комбо (кол-во дней бесперерывной активности) пользователя
    /// </summary>
    /// <param name="defaultYearStatistic"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void MaximumSeriesHander(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB)
    {
        List<int> orderedDays = responseFromDB
            .Select(x => x.AnsweredAt.DayOfYear)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
        
        ICollection<int> pastDays = [];
        int maxLength = 0;
        foreach (int day in orderedDays)
        {
            if (pastDays.Contains(day))
            {
                continue;
            }
            int currentDay = day;
            int currentLength = 1;

            // Ищем следующую часть последовательности
            while (orderedDays.Contains(currentDay + 1))
            {
                currentDay++;
                currentLength++;
                pastDays.Add(currentDay);
            }

            maxLength = Math.Max(maxLength, currentLength);
        }
        
        defaultYearStatistic.MaximumSeries = maxLength; 
    }
}

