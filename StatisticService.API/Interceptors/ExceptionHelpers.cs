using Grpc.Core;

namespace StatisticService.API.Interceptors
{
    public static class ExceptionHelpers
    {
        public static RpcException Handle<T>(this Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId) =>
            exception switch
            {
                TimeoutException => HandleTimeoutException((TimeoutException)exception, context, logger, correlationId),
                RpcException => HandleRpcException((RpcException)exception, logger, correlationId),
                _ => HandleDefault(exception, context, logger, correlationId)
            };

        private static RpcException HandleTimeoutException<T>(TimeoutException exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            logger.LogError(exception, $"CorrelationId: {correlationId} - Превышено максимальное время ожидания");

            var status = new Status(StatusCode.Internal, "Внешний ресурс не ответил в установленный срок");

            return new RpcException(status, CreateTrailers(correlationId));
        }

        private static RpcException HandleRpcException<T>(RpcException exception, ILogger<T> logger, Guid correlationId)
        {
            logger.LogError(exception, $"CorrelationId: {correlationId} - Произошла ошибка");
            var trailers = exception.Trailers;
            trailers.Add(CreateTrailers(correlationId)[0]);
            return new RpcException(new Status(exception.StatusCode, exception.Message), trailers);
        }

        private static RpcException HandleDefault<T>(Exception exception, ServerCallContext context, ILogger<T> logger, Guid correlationId)
        {
            logger.LogError(exception, $"CorrelationId: {correlationId} - Произошла ошибка");
            return new RpcException(new Status(StatusCode.Internal, exception.Message), CreateTrailers(correlationId));
        }

        /// <summary>
        ///  Adding the correlation to Response Trailers
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        private static Metadata CreateTrailers(Guid correlationId)
        {
            Metadata trailers = new()
            {
                { "CorrelationId", correlationId.ToString() }
            };
            return trailers;
        }
    }

}
