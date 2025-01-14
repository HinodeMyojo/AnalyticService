using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using StatisticService.API.Infrastructure;
using StatisticService.BLL.Abstractions.Service;
using StatisticService.BLL.Dto;
using StatisticService.BLL.Dto.YearStatistic;
using StatisticService.BLL.Entity;

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

        public override async Task<GetLastActivityResponse> GetLastActivity(GetLastActivityRequest request, ServerCallContext context)
        {
            List<StatisticEntity> lastActivityFromDatabase = await _service.GetLastActivity(request.UserId);

            GetLastActivityResponse result = new()
            {};

            foreach (StatisticEntity item in lastActivityFromDatabase)
            {
                result.Data.Add(new GetLastActivityModel()
                {
                    ModuleId = item.ModuleId,
                    CompletedAt = item.AnsweredAt.ToTimestamp()
                });
            }

            return result;
        }

        /// <summary>
        /// Метод для проверки связи между серверами
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<PingResponse> Ping(PingRequest request, ServerCallContext context)
        {
            return new PingResponse()
            {
                Message = "Ground Control to Major Tom!"
            };
        }

        /// <summary>
        /// Метод для сохранения статистики
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<StatisticResponse> SaveStatistic(StatisticRequest request, ServerCallContext context)
        {
            // Проверяем входящие данные
            Validator.AssertRequestStatistic(request);

            // Маппим в дто для дальнейшенй обработки в слоях
            RequestStatisticDto model = MapToRequestStatisticDto(request);

            int result = await _service.SaveStatisticAsync(model);

            return new StatisticResponse
            {
                Id  = result,
            };
        }

        public override async Task<GetStatisticByIdResponse> GetStatisticById(GetStatisticByIdRequest request, ServerCallContext context)
        {
            ResponseStatisticDto result = await _service.GetStatisticById(request.Id);
            return new GetStatisticByIdResponse
            {
                NumberOfAttempts = result.NumberOfAttempts,
                PercentSuccess = result.PercentSuccess,
                CompletedAt = result.CompletedAt.ToTimestamp(),
            };
        }

        public override Task<GetAwailableYearsResponse> GetAwailableYears(GetAwailableYearsRequest request, ServerCallContext context)
        {
            return base.GetAwailableYears(request, context);
        }

        /// <summary>
        /// Метод для получения информации об активности по заданному году
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<YearStatisticResponse> GetYearStatisic(YearStatisticRequest request, ServerCallContext context)
        {
            // Проверяем входящие данные
            Validator.AssertRequestYearStatistic(request);

            YearStatisticDto result = await _service.GetYearStatisticAsync(request.UserId, request.Year);

            YearStatisticResponse response = new()
            {
                Year = result.Year,
                ActiveDays = result.ActiveDays,
                MaximumSeries = result.MaximumSeries,
                NumberOfActions = result.NumberOfActions,
                Colspan = { result.Colspan },
            };

            response.Data.AddRange(MapToYearStatisticRows(result.Data));

            return response;
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

        private static IEnumerable<YearStatisticRow> MapToYearStatisticRows(YearStatisticData[][] data) =>
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
