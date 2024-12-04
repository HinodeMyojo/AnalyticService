namespace StatisticService.BLL.Dto.YearStatistic
{
    public class YearStatisticModel
    {
        public int Year { get; set; }
        public List<YearStatisticData> Data { get; set; } = [];
    }
}
