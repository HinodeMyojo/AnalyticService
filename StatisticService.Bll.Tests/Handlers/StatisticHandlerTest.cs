using StatisticService.BLL.Dto.YearStatistic;
using StatisticService.BLL.Entity;
using StatisticService.BLL.Handlers;

namespace StatisticService.Bll.Tests.Handlers;

public sealed class StatisticHandlerTest
{
    private readonly IStatisticHandler _statisticHandler;

    public StatisticHandlerTest()
    {
        _statisticHandler = new StatisticHandler(); // Используем реализацию
    }

    [Theory]
    [MemberData(nameof(GetTestCasesForMaximumSeries))]
    public void MaximumSeriesHander_ReturnsMaximumSeriesCorrectly(
    IEnumerable<StatisticEntity> responseFromDb, int expectedMaxSeries)
    {
        // Arrange
        YearStatisticDto mockDefaultYearStatistic = new();
        
        // Act
        _statisticHandler.MaximumSeriesHander(mockDefaultYearStatistic, responseFromDb);
        
        // Assert
        Assert.Equal(expectedMaxSeries, mockDefaultYearStatistic.MaximumSeries);
    }

    public static IEnumerable<object[]> GetTestCasesForMaximumSeries()
    {
       yield return
       [
           // Test Case 1: Простая последовательность
            new List<StatisticEntity>
            {
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 1) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 2) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 3) }
            },
            3 // Самая длинная последовательность: 1, 2, 3
       ];

        yield return
        [
            // Test Case 2: Прерывистая последовательность
            new List<StatisticEntity>
            {
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 1) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 3) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 5) }
            },
            1 // Самая длинная последовательность: только один день подряд
        ];

        yield return
        [
            // Test Case 3: Повторяющиеся дни
            new List<StatisticEntity>
            {
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 1) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 1) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 2) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 3) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 3) }
            },
            3 // Самая длинная последовательность: 1, 2, 3
        ];

        yield return
        [
            // Test Case 4: Пустой список
            new List<StatisticEntity>(),
            0 // Нет данных
        ];

        yield return
        [
            // Test Case 5: Один день
            new List<StatisticEntity>
            {
                new StatisticEntity { AnsweredAt = new DateTime(2025, 1, 1) }
            },
            1 // Только один день
        ];
        
        yield return
        [
            // Test Case 6: Проверка концов года
            new List<StatisticEntity>
            {
                new StatisticEntity { AnsweredAt = new DateTime(2025, 12, 27) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 12, 29) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 12, 30) },
                new StatisticEntity { AnsweredAt = new DateTime(2025, 12, 31) },
            },
            3 // Три дня
        ];
    }
}