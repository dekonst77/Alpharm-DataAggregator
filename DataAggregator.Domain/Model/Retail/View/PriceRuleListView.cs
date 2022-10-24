using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.View
{
    [Table("PriceRuleListView", Schema = "calc")]
    public class PriceRuleListView
    {
        [Key]
        [Column(Order = 1)]
        public long PriceRuleId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int Year { get; set; }
        [Key]
        [Column(Order = 3)]
        public int Month { get; set; }
        

        /// <summary>
        /// Дата изменения правила
        /// </summary>
        public DateTime? Date { get; set; }
        public string RegionCode { get; set; }
        public string RegionFullName { get; set; }
        public decimal? SellingPriceMin { get; set; }
        public decimal? SellingPriceMax { get; set; }
        public decimal? PurchasePriceMin { get; set; }
        public decimal? PurchasePriceMax { get; set; }
        public int? ClassifierId { get; set; }
        public string TradeName { get; set; }
        public string Brand { get; set; }
        public string DrugDescription { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public Guid UserId { get; set; }
        public string Surname { get; set; }
        public string Comment { get; set; }

        public bool InUsed { get; set; }
        public bool OutUsed { get; set; }
    }
}
