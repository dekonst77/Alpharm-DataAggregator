using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.Systematization.View;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("ClassificationGeneric", Schema = "Classifier")]
    public class ClassificationGeneric
    {
        public  int Id { get; set; }
        public long TradeNameId { get; set; }
        public long INNGroupId { get; set; }
        public string UserId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long? GenericId { get; set; }
        public virtual Generic Generic { get; set; }
        public virtual TradeName TradeName { get; set; }
        public virtual INNGroup INNGroup { get; set; }
        public virtual Manufacturer OwnerTradeMark { get; set; }
        public DateTime? DateEdit { get; set; }
        public virtual User User { get; set; }
      
    }

}
