using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace DataAggregator.Domain.Model.Retail.CTM
{
    [Table("CTMView", Schema = "Classifier")]
    public class CTMView
    {
        public long Id { get; set; }
        public long ClassifierID { get; set; }
        public long TradeNameId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public long? NetworkID { get; set; }
        public string NetworkName { get; set; }
        public bool? DeBrikingState { get; set; }
        public string DeBrikingStateName { get; set; }
        public DateTime? Period { get; set; }
        public string Comment { get; set; }
        public DateTime? Date_modifired { get; set; }
    }

    public class Network
    {
        public long? NetworkID { get; set; }
        public string NetworkName { get; set; }
        public DateTime Period { get; set; }
    }
}
