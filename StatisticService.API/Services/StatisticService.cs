using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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
            AssertRequestStatistic(request);

            // Маппим в дто для дальнейшенй обработки в слоях
            RequestStatisticDto model = MapToRequestStatisticDto(request);
            var result = await _service.SaveStatisticAsync(model);

            return new StatisticResponse
            {
                PercentSuccess = result.PercentSuccess,
                NumberOfAttempts = result.NumberOfAttempts
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
            AssertRequestYearStatistic(request);

            var result = await _service.GetYearStatisticAsync(request.UserId, request.Year);

            return new YearStatisticResponse
            {
                Year = result.Year,
                Colspan = { result.Colspan }
            };
        }

        /// <summary>
        /// Проверочный метод
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <exception cref="RpcException"></exception>
        private static void AssertRequestStatistic(StatisticRequest request)
        {
            if (request.ModuleId == 0 || request.UserId == 0)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Id пользователя и/или Id модуля не должны равняться нулю!"));
            }
        }

        /// <summary>
        /// Проверочный метод для GetYear
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <exception cref="RpcException"></exception>
        private static void AssertRequestYearStatistic(YearStatisticRequest request)
        {
            if (request.UserId == 0 || request.Year == 0)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Id пользователя и/или год не должны равняться нулю!"));
            }
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
