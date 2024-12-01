namespace StatisticService.BLL.Dto
{
    public class RequestStatisticDto
    {
        public int ModuleId {  get; set; }
        public int UserId { get; set; }
        public ICollection<RequestElementDto> Elements { get; set; } = [];
        public DateTime CompletedAt { get; set; }
    }

    
}
