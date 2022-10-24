using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Stat
{
    [Table("DateStat", Schema = "Stat")]
    public class DateStat
    {
        [Key]
        public string date { get; set; }
        public long PeriodId { get; set; }
        public long SourceId { get; set; }
        public long ToWorkCount { get; set; }
        public long ToAddingCount { get; set; }
        public long ToCheckingCount { get; set; }
        /// <summary>
        /// Сделано для фильтра загрузки данных в модуле привязки, с целью отметить галочкой данное поле
        /// </summary>
        [NotMapped]
        public bool IsChecked { get; set; }
    }
}
