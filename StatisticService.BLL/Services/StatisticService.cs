using StatisticService.BLL.Abstractions;
using StatisticService.BLL.Dto;
using StatisticService.DAL.Repository;

namespace StatisticService.BLL.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _repository;

        public Task<ResponseYearStatisticDto> GetYearStatisticAsync(int userId, int year)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseStatisticDto> SaveStatisticAsync(RequestStatisticDto model)
        {
            throw new NotImplementedException();
        }
    }
}
