using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Stat
{
    [Table("UserWorkStat", Schema = "Stat")]
    public class UserWorkStat
    {
        public long SourceId { get; set; }
        public long PeriodId { get; set; }
        [Key]
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public long InWorkCount { get; set; }
        public long ReadyCount { get; set; }

        /// <summary>
        /// Сделано для фильтра загрузки данных в модуле привязки, с целью отметить галочкой данное поле
        /// </summary>
        [NotMapped]
        public bool IsChecked { get; set; }
    }
}