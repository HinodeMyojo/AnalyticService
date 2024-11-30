namespace StatisticService.BLL.Dto
{
    public class StatisticDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public int AttemptCount { get; set; }
        public DateTime AnsweredAt { get; set; }
        public List<ElementStatisticDto> Elements { get; set; } = [];
    }
}
