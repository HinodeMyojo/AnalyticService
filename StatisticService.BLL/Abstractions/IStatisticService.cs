namespace StatisticService.BLL.Abstractions
{
    public interface IStatisticService
    {
        Task<StatisticResponse> SaveStatistic(StatisticRequest request);
    }
}