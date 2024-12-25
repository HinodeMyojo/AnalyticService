namespace StatisticService.BLL.Dto
{
    public class ResponseLastActivityDto
    {
        public IEnumerable<ResponseLastActivityModel> Data { get; set; } = [];
    }

    public class ResponseLastActivityModel
    {
        public int ModuleId { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
