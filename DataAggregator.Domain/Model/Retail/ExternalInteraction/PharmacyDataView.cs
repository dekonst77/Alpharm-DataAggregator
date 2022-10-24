using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail.ExternalInteraction
{
    [Table("PharmacyDataView", Schema = "external2")]
    public class PharmacyDataView
    {
        public Guid Id { get; set;}

        public string RegionCode { get; set; }

        public long PharmacyId { get; set; }

        
        public long? TradeNameId { get; set; }

        
        public string TradeName { get; set; }

        
        public long? DosageGroupId { get; set; }

        
        public string DosageGroup { get; set; }

        
        public long? FormProductId { get; set; }

        
        public string FormProduct { get; set; }


        public long? INNGroupId { get; set; }


        public string INNGroup { get; set; }


        public long? ConsumerPackingCount { get; set; }


        public long? FTGId { get; set; }


        public string FTG { get; set; }


        public long? ATCWhoId { get; set; }


        public string ATCWhoCode { get; set; }


        public string ATCWhoDescription { get; set; }


        public long? OwnerTradeMarkId { get; set; }


        public string OwnerTradeMark { get; set; }

        
        public bool IsRx { get; set; }

        
        public bool IsOtc { get; set; }

        
        public bool IsOther { get; set; }
        public Int16 type { get; set; }
        
        public bool IsBad { get; set; }

        
        public bool FederalBenefit { get; set; }

        
        public bool RegionalBenefit { get; set; }
    
        
        public decimal? SellingCount { get; set; }

        
        public decimal? SellingPrice { get; set; }

        
        public decimal? SellingSum { get; set; }

        
        public decimal? PurchaseCount { get; set; }

        
        public decimal? PurchasePrice { get; set; }

        
        public decimal? PurchaseSum { get; set; }
      
    }
}
