using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Stat
{
    [Table("RobotStat", Schema = "Stat")]
    public class RobotStat
    {
        public long SourceId { get; set; }
        public long PeriodId { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public long ReadyCount { get; set; }
        public long InWorkCount { get; set; }
        /// <summary>
        /// Сделано для фильтра загрузки данных в модуле привязки, с целью отметить галочкой данное поле
        /// </summary>
        [NotMapped]
        public bool IsChecked { get; set; } 
    }
}