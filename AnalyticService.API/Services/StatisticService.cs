using Grpc.Core;
using StatisticService.BLL.Abstractions;

namespace StatisticService.API.Services
{
    public class StatisticService : Statistic.StatisticBase
    {
        private readonly IStatisticService _service;

        public override async Task<StatisticResponse> SaveStatistic(StatisticRequest request, ServerCallContext context)
        {
            var result = await _service.SaveStatisticAsync(
                moduleId: request.ModuleId,
                userId: request.UserId,
                elements: request.Elements.Select(e => new StatisticElementDto
                {
                    ElementId = e.ElementId,
                    Answer = e.Answer
                }),
                completedAt: request.CompletedAt.ToDateTime()
            );

            return new StatisticResponse
            {
                PercentSuccess = result.PercentSuccess,
                NumberOfAttempts = result.NumberOfAttempts
            };
        }

        public override async Task<YearStatisticResponse> GetYearStatistic(YearStatisticRequest request, ServerCallContext context)
        {
            var result = await _service.GetYearStatisticAsync(request.UserId, request.Year);

            var response = new YearStatisticResponse
            {
                Year = result.Year,
                Colspan = { result.Colspan },
                Data = {
                result.Data.Select(row => new YearStatisticRow
                {
                    Values = { row.Values }
                })
            }
            };

            return response;
        }
    }
}
