using StatisticService.BLL.Abstractions;
using StatisticService.BLL.Dto;
using StatisticService.DAL.Repository;

namespace StatisticService.BLL.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IStatisticRepository _repository;

        public Task<YearStatisticDto> GetYearStatisticAsync(int userId, int year)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseStatisticModel> SaveStatisticAsync(int moduleId, int userId, object elements, DateTime completedAt)
        {
            throw new NotImplementedException();
        }
    }
}
