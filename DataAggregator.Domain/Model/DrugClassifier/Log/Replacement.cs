using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Log
{
    [Table("Replacement", Schema = "Log")]
    public class Replacement
    {
        public long Id { get; set; }
        public long FromDrugId { get; set; }
        public long FromOwnerTradeMarkId { get; set; }
        public long FromPackerId { get; set; }
        public long ToDrugId { get; set; }
        public long ToOwnerTradeMarkId { get; set; }
        public long ToPacekrId { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }  
    }
}