﻿namespace StatisticService.BLL.Entity
{
    /// <summary>
    /// Дефолтная статистика по годам
    /// </summary>
    public class DefaultYearStatisticEntity
    {
        public int Id { get; set; }
        public required int Year { get; set; }
        public required string YearStatistic {  get; set; }
        public required List<int> Colspan {  get; set; }
    }
}
