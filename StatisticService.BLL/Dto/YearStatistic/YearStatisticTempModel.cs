namespace StatisticService.BLL.Dto.YearStatistic
{
    public class YearStatisticTempModel
    {
        public int Year { get; set; }
        public List<YearStatisticData> Data { get; set; } = [];
    }
}
