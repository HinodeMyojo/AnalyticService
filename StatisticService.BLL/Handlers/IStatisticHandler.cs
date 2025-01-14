using StatisticService.BLL.Dto.YearStatistic;
using StatisticService.BLL.Entity;

namespace StatisticService.BLL.Handlers;

public interface IStatisticHandler
{
    public void DataHandler(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB);
    public void ActiveDaysHander(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB);
    public void NumberOfActionsHandler(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB);
    public void MaximumSeriesHander(YearStatisticDto defaultYearStatistic, IEnumerable<StatisticEntity> responseFromDB);
}