using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using StatisticService.BLL.Abstractions;
using StatisticService.BLL.Dto;

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
                elements: request.Elements.Select(e => new ElementStatisticDto
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

        public override async Task<YearStatisticResponse> GetYearStatisic(YearStatisticRequest request, ServerCallContext context)
        {
            YearStatisticDto result = await _service.GetYearStatisticAsync(request.UserId, request.Year);

            YearStatisticResponse response = new YearStatisticResponse
            {
                Year = result.Year,
                Colspan = { result.Colspan }
            };

            foreach (YearStatisticData[] row in result.Data)
            {
                // Создаём новую строку YearStatisticRow
                YearStatisticRow statisticRow = new();

                foreach (YearStatisticData item in row)
                {
                    // Добавляем каждую модель в строку
                    statisticRow.Values.Add(new YearStatisticModel
                    {
                        Date = item.Date.ToTimestamp(),
                        Value = item.Value
                    });
                }

                response.Data.Add(statisticRow);
            }

            return response;
        }
    }
}
