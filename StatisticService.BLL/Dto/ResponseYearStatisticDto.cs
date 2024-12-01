namespace StatisticService.BLL.Dto
{
    public class ResponseYearStatisticDto
    {
        public int Year { get; set; }
        public ResponseYearStatisticData[][] Data { get; set; } = [];
        public List<int> Colspan { get; set; } = [];
    }

    public class ResponseYearStatisticData
    {
        public DateTime Date { get; set; }
        public int Value { get; set; }
    }
}
