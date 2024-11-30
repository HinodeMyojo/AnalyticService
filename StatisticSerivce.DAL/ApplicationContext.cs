using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StatisticService.DAL.Entity;

namespace StatisticService.DAL
{
    public class ApplicationContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<StatisticEntity> StatisticEntities { get; set; }
        public DbSet<ElementStatisticEntity> ElementStatisticEntities { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
