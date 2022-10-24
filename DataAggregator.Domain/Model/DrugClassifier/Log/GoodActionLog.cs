using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Log
{
    [Table("GoodsActionLog", Schema = "Log")]
    public class GoodsActionLog
    {
        public long Id { get; set; }
        public long ActionId { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
        public long GoodsProductionInfoId { get; set; }
        //Поддержка IsActual осуществляется на уровне тригера
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool IsActual { get; set; }
    }
}
