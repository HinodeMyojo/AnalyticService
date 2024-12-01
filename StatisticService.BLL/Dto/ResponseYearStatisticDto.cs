namespace StatisticService.BLL.Dto
{
    public class ResponseYearStatisticDto
    {
        public int Year { get; set; }
        public ResponseYearStatisticData[][] Data { get; set; } = [];
        public List<int> Colspan { get; set; } = [];
    }

    
}
