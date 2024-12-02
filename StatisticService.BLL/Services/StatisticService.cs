using StatisticService.BLL.Abstractions;
using StatisticService.BLL.Dto;
using StatisticService.BLL.Entity;
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

        public async Task<ResponseStatisticDto> SaveStatisticAsync(RequestStatisticDto model)
        {

            int attemptCount = await FindAttemptCountModuleByUser(model.ModuleId, model.UserId);

            StatisticEntity entity = new()
            {
                AnsweredAt = model.CompletedAt,
                AttemptCount = 1
            };

            return new ResponseStatisticDto { NumberOfAttempts = attemptCount, PercentSuccess = 0 }; 
        }

        private async static Task<int> FindAttemptCountModuleByUser(int moduleId, int userId)
        {
            return 1;
        }
    }
}
