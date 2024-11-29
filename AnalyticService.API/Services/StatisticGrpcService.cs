using Grpc.Core;
using StatisticService.BLL.Abstractions;

namespace StatisticService.API.Services
{
    public class StatisticGrpcService : Statistic.StatisticBase
    {
        private readonly IStatisticService _service;

        public override async Task<StatisticResponse> SaveStatistic(StatisticRequest request, ServerCallContext context)
        {

            StatisticResponse result = await _service.SaveStatistic(request);

            return result;
        }

        public override Task<YearStatisticResponse> GetYearStatisic(YearStatisticRequest request, ServerCallContext context)
        {
            return base.GetYearStatisic(request, context);
        }
    }
}
