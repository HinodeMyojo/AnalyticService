namespace StatisticService.BLL.Entity
{
    /// <summary>
    /// Дефолтная статистика по годам
    /// </summary>
    public class DefaultYearStatisticEntity
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public required string YearStatistic {  get; set; }
    }
}
