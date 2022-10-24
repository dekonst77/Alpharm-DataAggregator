using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Stat
{
     [Table("DataTypeStat", Schema = "Stat")]
    public class DataTypeStat
    {
        [Key]
        public long OrderId { get; set; }
        public string FullName { get; set; }
        public long PeriodId { get; set; }
        public long SourceId { get; set; }
        public long ToWorkCount { get; set; }
        public long InWorkCount { get; set; }
        public long ReadyCount { get; set; }

        /// <summary>
        /// Сделано для фильтра загрузки данных в модуле привязки, с целью отметить галочкой данное поле
        /// </summary>
        [NotMapped]
        public bool IsChecked { get; set; }
    }
}