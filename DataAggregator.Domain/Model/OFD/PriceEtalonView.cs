using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.OFD
{
    [Table("PriceEtalonView", Schema = "dbo")]
    public class PriceEtalonView
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public long ClassifierId { get; set; }
        public decimal Price { get; set; }
        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public string TradeName { get; set; }
        public string Brand { get; set; }
    }
}
