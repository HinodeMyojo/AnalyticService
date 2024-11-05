using Grpc.Core;

namespace AnalyticService.API.Services
{
    public class DataAnalyticService : DataAnalytic.DataAnalyticBase
    {
        public override Task<AnalyticsResponse> GetAnalytics(AnalyticsRequest request, ServerCallContext context)
        {
            var response = new AnalyticsResponse
            {
                Result = $"Аналитика для ID: {request.Id}",
                Details = "Подробная информация о запрашиваемой аналитике"
            };

            return Task.FromResult(response);
        }
    }
}
