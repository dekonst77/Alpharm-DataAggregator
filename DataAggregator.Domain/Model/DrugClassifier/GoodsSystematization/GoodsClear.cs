using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("GoodsClear", Schema = "GoodsSystematization")]
    public class GoodsClear
    {
        public long Id { get; set; }

        public long DrugClearId { get; set; }

        public DateTime DateAdd { get; set; }

        public long SourcePeriodId { get;set; }

        public long? GoodsCategoryId { get; set; }

        public DateTime? GoodsCategoryDate { get; set; }

        public Guid? GoodsCategoryUserId { get; set; }
    }
}
