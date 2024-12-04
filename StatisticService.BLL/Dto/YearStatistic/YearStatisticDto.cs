namespace StatisticService.BLL.Dto.YearStatistic
{
    public class YearStatisticDto
    {
        public int Year { get; set; }
        public YearStatisticData[][] Data { get; set; } = [];
        public List<int> Colspan { get; set; } = [];
    }


}
