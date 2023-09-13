using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("ClassifierHistoryView", Schema = "Log")]
    public class ClassifierHistoryView
    {
        public int Id { get; set; }
        public int ClassifierId { get; set; }
        public int DrugId { get; set; }
        public int OwnerTradeMarkId { get; set; }
        public int PackerId { get; set; }
        public string Who { get; set; }
        public string What { get; set; }
        public DateTime When { get; set; }
        public string Flag { get; set; }
    }
}