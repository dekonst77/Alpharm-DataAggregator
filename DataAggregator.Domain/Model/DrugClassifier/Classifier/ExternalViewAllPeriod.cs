using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("ExternalViewAllPeriod", Schema = "stable")]
    public class ExternalViewAllPeriod
    {
        [Key]
        public long ClassifierId { get; set; }

        public long? TradeNameId { get; set; }
        public string TradeName { get; set; }

        public long? DrugId { get; set; }
        public string DrugDescription { get; set; }

        public long? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }

        public long? PackerId { get; set; }
        public string Packer { get; set; }

        public long? BrandId { get; set; }
    }
}