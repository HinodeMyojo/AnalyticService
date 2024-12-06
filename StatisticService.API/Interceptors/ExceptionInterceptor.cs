using Grpc.Core;
using Grpc.Core.Interceptors;

namespace StatisticService.API.Interceptors
{
    public class ExceptionInterceptor : Interceptor
    {
        private readonly ILogger<ExceptionInterceptor> _logger;
        private readonly Guid _correlationId;

        public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger)
        {
            _logger = logger;
            _correlationId = Guid.NewGuid();
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception e)
            {
                throw e.Handle(context, _logger, _correlationId);
            }
        }


        //public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        //    IAsyncStreamReader<TRequest> requestStream,
        //    ServerCallContext context,
        //    ClientStreamingServerMethod<TRequest, TResponse> continuation)
        //{
        //    return await continuation(requestStream, context);
        //}


        //public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        //    TRequest request,
        //    IServerStreamWriter<TResponse> responseStream,
        //    ServerCallContext context,
        //    ServerStreamingServerMethod<TRequest, TResponse> continuation)
        //{
        //    await continuation(request, responseStream, context);
        //}

        //public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        //    IAsyncStreamReader<TRequest> requestStream,
        //    IServerStreamWriter<TResponse> responseStream,
        //    ServerCallContext context,
        //    DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        //{
        //    await continuation(requestStream, responseStream, context);

        //}
    }
}
