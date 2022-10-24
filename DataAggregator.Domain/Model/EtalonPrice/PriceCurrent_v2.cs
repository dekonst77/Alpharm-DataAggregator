using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.EtalonPrice
{
    [Table("PriceCurrent_v2", Schema = "EtalonPrice")]
    public class PriceCurrent_v2
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ClassifierId { get; set; }    
        public decimal? PriceNew { get; set; }      
        public string Comment { get; set; }
        public DateTime? DateUpdate { get; set; }
        public Guid? UserIdUpdate { get; set; }
        public bool WithoutPrice { get; set; }    

        public bool ForChecking { get; set; }
    }
    
}
