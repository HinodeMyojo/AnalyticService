namespace StatisticService.BLL.Dto
{
    public class YearStatisticDto
    {
        public int Year { get; set; }
        public YearStatisticData[][] Data { get; set; } = [];
        public List<int> Colspan { get; set; } = [];
    }

    public class YearStatisticData
    {
        public DateTime Date { get; set; }
        public int Value { get; set; }
    }
}
