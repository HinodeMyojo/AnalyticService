using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Npgsql.Internal;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace StatisticService.API.Middleware
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IHostEnvironment env;
        private const string UnhandledExceptionMsg = "Произошло что-то непонятное на сервере статистики.";

        private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public ExceptionHandler(IHostEnvironment env)
        {
            this.env = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails;
            StatusCode statusCode;

            switch (exception)
            {
                //case FormatException ex:
                //    statusCode = StatusCodes.Status400BadRequest;
                //    problemDetails = CreateProblemDetails(httpContext, ex, statusCode);
                //    break;
                //case ConnectionException ex:
                //    statusCode = StatusCodes.Status503ServiceUnavailable;
                //    problemDetails = CreateProblemDetails(httpContext, ex, statusCode);
                //    break;
                //case ArgumentNullException ex:
                //    statusCode = StatusCodes.Status404NotFound;
                //    problemDetails = CreateProblemDetails(httpContext, ex, statusCode);
                //    break;
                default:
                    statusCode = StatusCode.Unknown;
                    problemDetails = CreateProblemDetails(httpContext, exception, statusCode);
                    break;
            }

            //var json = ToJson(problemDetails);

            const string contentType = "application/problem+json";
            httpContext.Response.ContentType = contentType;

            throw new RpcException(new Status(statusCode, problemDetails.Detail));

            return true;
        }

        private ProblemDetails CreateProblemDetails(in HttpContext context, in Exception exception, StatusCode statusCode)
        {
            var reasonPhrase = exception.Message;
            if (string.IsNullOrEmpty(reasonPhrase))
            {
                reasonPhrase = UnhandledExceptionMsg;
            }

            var problemDetails = new ProblemDetails
            {
                Status = ((int)statusCode),
                Title = reasonPhrase,
            };

            // в прод допускаем только короткую фразу
            if (env.IsProduction())
            {
                return problemDetails;
            }

            problemDetails.Detail = exception.ToString();
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
            problemDetails.Extensions["data"] = exception.Data;

            return problemDetails;
        }

        //private string ToJson(in ProblemDetails problemDetails)
        //{
        //    try
        //    {
        //        return JsonSerializer.Serialize(problemDetails, SerializerOptions);
        //    }
        //    catch (Exception ex)
        //    {
        //        //const string msg = "An exception has occurred while serializing error to JSON";
        //        //_logger.LogError(ex, msg);
        //    }

        //    return string.Empty;
        //}
    }
}
