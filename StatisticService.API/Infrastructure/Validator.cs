using Grpc.Core;

namespace StatisticService.API.Infrastructure
{
    public static class Validator
    {
        /// <summary>
        /// Проверочный метод для SaveYearStatistic
        /// </summary>
        /// <param name="request"></param>
        /// <exception cref="RpcException"></exception>
        public static void AssertRequestStatistic(StatisticRequest request)
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
        /// <param name="request"></param
        /// <exception cref="RpcException"></exception>
        public static void AssertRequestYearStatistic(YearStatisticRequest request)
        {
            if (request.UserId == 0 || request.Year == 0)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Id пользователя и/или год не должны равняться нулю!"));
            }
        }
    }
}
