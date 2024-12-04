namespace StatisticService.BLL.Dto.YearStatistic
{
    public class ResponseYearStatisticDto
    {
        public int Year { get; set; }
        public YearStatisticData[][] Data { get; set; } = [];
        public List<int> Colspan { get; set; } = [];
    }


}
