using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.EtalonPrice

{
    [Table("PriceCurrentView_v2", Schema = "EtalonPrice")]
    public class PriceCurrentView_v2
    {
        [Key]
        public int ClassifierId { get; set; }
        public int? TradeNameId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public int? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public int? PackerId { get; set; }
        public string Packer { get; set; }
        public int? OwnerRegistrationCertificateId { get; set; }
        public string OwnerRegistrationCertificate { get; set; }
        public int? RetailDataSellingSum { get; set; }
        public decimal? PriceOld { get; set; }
        public decimal? PriceNew { get; set; }
        public int? RetailDataMedianaPrice { get; set; }
        public int? OFDMediana50Price { get; set; }
        public int? OFDMediana65Price { get; set; }    
        public string Comment { get; set; }
        public int? INNGroupId { get; set; }
        public string INNGroup { get; set; }
        public DateTime? DateUpdate { get; set; }
        public Guid? UserIdUpdate { get; set; }
        public string User { get; set; }
        public bool WithoutPrice { get; set; }
        public string Type { get; set; }
        public decimal? PriceVED { get; set; }
        public long? OFDSum { get; set; }
        public bool ForChecking { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public int? Web1Mediana50Price { get; set; }
        public int? Web1Mediana65Price { get; set; }
        public int? Web2Mediana50Price { get; set; }
        public int? Web2Mediana65Price { get; set; }
        public int? Web3Mediana50Price { get; set; }
        public int? Web3Mediana65Price { get; set; }
        public int? Web4Mediana50Price { get; set; }
        public int? Web4Mediana65Price { get; set; }
      
        public int? ProcentOFDAB { get; set; }

        public int? ProcentOFD { get; set; }

        public int? ProcentAB { get; set; }

        public int Sum30k { get; set; }
    }

}
