namespace StatisticService.BLL.Entity
{
    public class StatisticEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ModuleId {  get; set; }
        public int AttemptCount {  get; set; }
        public DateTime AnsweredAt {  get; set; }
        public List<ElementStatisticEntity> Elements { get; set; } = [];
    }
}
