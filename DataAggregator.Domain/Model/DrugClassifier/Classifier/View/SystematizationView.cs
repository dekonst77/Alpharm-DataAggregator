using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class SystematizationView
    {
        [Key]
        public Guid Guid { get; set; }
        public long? ClassifierId { get; set; }
        public long? DrugId { get; set; }
        public long? TradeNameId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public long? INNGroupId { get; set; }
        public string INNGroup { get; set; }
        public long? PackerId { get; set; }
        public string Packer { get; set; }
        public int? RealPackingCount { get; set; }
        public int? ConsumerPackingCount { get; set; }
        public string Comment { get; set; }
        public bool Used { get; set; }
        [JsonIgnore]
        public string FormProduct { get; set; }
        public long? FormProductId { get; set; }
        [JsonIgnore]
        public string DosageGroup { get; set; }
        public long? DosageGroupId { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public bool? RegistrationCertificateIsBlocked { get; set; }
        public decimal? Price { get; set; }
    }


    public class SystematizationView_LPDOP
    {
        public long? ClassifierId { get; set; }
        public long? DrugId { get; set; }
        public long? GoodsId { get; set; }
        public string TradeName { get; set; }
        public string INNGroup { get; set; }
        public string DrugDescription { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public long? PackerId { get; set; }
        public string Packer { get; set; }
        public string Comment { get; set; }
        public bool Used { get; set; }
        public bool? ToRetail { get; set; }
        public bool IsOther { get; set; }
        public byte? GoodsCategoryId { get; set; }
        public int? RealPackingCount { get; set; }
        public int? ConsumerPackingCount { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public string Price { get; set; }
    }

    [Table("ExternalView_FULL", Schema = "Classifier")]
    public class ExternalView_FULL
    {
        [Key]
        public long ClassifierId { get; set; }
        public string TradeName { get; set; }
        public string INNGroup { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public bool Used { get; set; }
        public bool IsOther { get; set; }

        public byte? GoodsCategoryId { get; set; }
        public string GoodsCategoryName { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public decimal? Price { get; set; }
    }
}