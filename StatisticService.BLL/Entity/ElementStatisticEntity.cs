namespace StatisticService.BLL.Entity
{
    public class ElementStatisticEntity
    {
        public int Id { get; set; }
        public StatisticEntity? Statistic { get; set;}
        public int StatisticId { get; set; }
        public int ElementId { get; set; }
        public bool Answer {  get; set; }
    }
}
