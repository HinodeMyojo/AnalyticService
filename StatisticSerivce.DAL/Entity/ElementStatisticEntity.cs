namespace StatisticService.DAL.Entity
{
    public class ElementStatisticEntity
    {
        public StatisticEntity? Statistic { get; set;}
        public int StatisticId { get; set; }
        public int ElementId { get; set; }
        public bool Answer {  get; set; }
    }
}
