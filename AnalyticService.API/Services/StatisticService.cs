using Grpc.Core;

namespace StatisticService.API.Services
{
    public class StatisticService : Statistic.StatisticBase
    {
        public override Task<StatisticResponse> SetStatistic(StatisticRequest request, ServerCallContext context)
        {

            return base.SetStatistic(request, context);
        }

        public override Task<YearStatisticResponse> GetYearStatisic(YearStatisticRequest request, ServerCallContext context)
        {
            return base.GetYearStatisic(request, context);
        }
    }
}
