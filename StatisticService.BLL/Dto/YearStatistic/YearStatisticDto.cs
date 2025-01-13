namespace StatisticService.BLL.Dto.YearStatistic
{
    public class YearStatisticDto
    {
        public int Year { get; set; }
        public int NumberOfActions { get; set; }
        public int ActiveDays { get; set; }
        public int MaximumSeries { get; set; }
        public YearStatisticData[][] Data { get; set; } = [];
        public List<int> Colspan { get; set; } = [];
    }


}
