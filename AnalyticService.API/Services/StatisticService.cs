using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using StatisticService.BLL.Abstractions;
using StatisticService.BLL.Dto;

namespace StatisticService.API.Services
{
    //[Authorize]
    public class StatisticService : Statistic.StatisticBase
    {
        private readonly IStatisticService _service;

        public StatisticService(IStatisticService service)
        {
            _service = service;
        }

        public override async Task<StatisticResponse> SaveStatistic(StatisticRequest request, ServerCallContext context)
        {
            AssertStatistic(request, context);

            var model = MapToRequestStatisticDto(request);
            var result = await _service.SaveStatisticAsync(model);

            return new StatisticResponse
            {
                PercentSuccess = result.PercentSuccess,
                NumberOfAttempts = result.NumberOfAttempts
            };
        }

        public override async Task<YearStatisticResponse> GetYearStatisic(YearStatisticRequest request, ServerCallContext context)
        {
            var result = await _service.GetYearStatisticAsync(request.UserId, request.Year);

            return new YearStatisticResponse
            {
                Year = result.Year,
                Colspan = { result.Colspan },
                Data = { MapToYearStatisticRows(result.Data) }
            };
        }

        private void AssertStatistic(StatisticRequest request, ServerCallContext context)
        {
            
        }


        /// <summary>
        /// Вспомогательный метод по мапингу в dto
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static RequestStatisticDto MapToRequestStatisticDto(StatisticRequest request) =>
            new()
            {
                CompletedAt = request.CompletedAt.ToDateTime(),
                Elements = request.Elements.Select(x => new RequestElementDto
                {
                    Answer = x.Answer,
                    ElementId = x.ElementId,
                }).ToList(),
                ModuleId = request.ModuleId,
                UserId = request.UserId
            };

        private static IEnumerable<YearStatisticRow> MapToYearStatisticRows(ResponseYearStatisticData[][] data) =>
            data.Select(row => new YearStatisticRow
            {
                Values = { row.Select(item => new YearStatisticModel
                {
                    Date = item.Date.ToTimestamp(),
                    Value = item.Value
                }) }
            });
    }
}
