using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Stat
{
    [Table("DrugClearWorkStat", Schema = "Stat")]
    public class DrugClearWorkStat
    {
        public long SourceId { get; set; }
        public long PeriodId { get; set; }
        [Key]
        [Column(Order = 1)]
        public long ToWorkCount { get; set; }
        [Key]
        [Column(Order = 2)]
        public long InWorkCount { get; set; }
        [Key]
        [Column(Order = 3)]
        public long ReadyCount { get; set; }

        /// <summary>
        /// Сделано для фильтра загрузки данных в модуле привязки, с целью отметить галочкой данное пол
        /// </summary>
        [NotMapped]
        public bool IsChecked { get; set; }
    }
}