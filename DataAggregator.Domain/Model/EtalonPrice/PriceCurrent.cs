using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.EtalonPrice
{
    [Table("PriceCurrent", Schema = "EtalonPrice")]
    public class PriceCurrent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClassifierId { get; set; }
        public decimal? RetailDataSellingSum { get; set; }
        public decimal? PriceOld { get; set; }
        public decimal? PriceNew { get; set; }
        public decimal? PriceCalc { get; set; }
        public string StatusText { get; set; }
        public decimal? RetailDataAvgPrice { get; set; }
        public decimal? RetailDataMedianaPrice { get; set; }
        public decimal? OFDMediana50Price { get; set; }
        public decimal? OFDMediana65Price { get; set; }
        public decimal? RetailDataPrice { get; set; }
        public decimal? OFDPrice { get; set; }
        public string Comment { get; set; }
        public DateTime? DateUpdate { get; set; }
        public Guid? UserIdUpdate { get; set; }
        public int? PriceRawMedianaRaw { get; set; }
        public int? PriceOFDMedianaRaw { get; set; }
        public int? PriceRaw65PercentileOFD { get; set; }
        public int? PriceOFD65PercentileOFD { get; set; }
        public bool WithoutPrice { get; set; }
        public bool IsFractionalPackaging { get; set; }

        public bool ForChecking { get; set; }
    }
    
}
