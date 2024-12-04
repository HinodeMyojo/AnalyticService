namespace StatisticService.BLL.Dto
{
    public class ResponseStatisticDto
    {
        public int PercentSuccess {  get; set; }
        public int NumberOfAttempts {  get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
