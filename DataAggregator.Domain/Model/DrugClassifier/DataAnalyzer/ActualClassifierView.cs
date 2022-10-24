using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.DataAnalyzer
{
    [Table("ActualClassifierView", Schema = "Systematization")]
    public class ActualClassifierView
    {
        [Key]
        public Guid? Guid { get; set; }
        public long DrugId { get; set; }
        public long TradeNameId { get; set; }
        public string TradeName { get; set; }
        public long DosageGroupId { get; set; }
        public long InnGroupId { get; set; }
        public long FormProductId { get; set; }
        public int? ConsumerPackingCount { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
    }
}
