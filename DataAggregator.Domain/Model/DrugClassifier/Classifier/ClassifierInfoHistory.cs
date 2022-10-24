using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("ClassifierInfoHistory", Schema = "history")]
    public class ClassifierInfoHistory
    {
        public int Id { get; set; }
        public int ClassifierInfoId { get; set; }
        public int? DrugId { get; set; }
        public int? GoodsId { get; set; }
        public int OwnerTradeMarkId { get; set; }
        public int PackerId { get; set; }

    }
}
