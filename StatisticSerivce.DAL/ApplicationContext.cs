using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StatisticService.BLL.Entity;

namespace StatisticService.DAL
{
    public class ApplicationContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        
        public DbSet<StatisticEntity> StatisticEntities { get; set; }
        public DbSet<ElementStatisticEntity> ElementStatisticEntities { get; set; }
        public DbSet<DefaultYearStatisticEntity> DefaultYearStatisticEntities {  get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DefaultYearStatisticEntity>().HasIndex(x => x.Year).IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

        }
    }
}
