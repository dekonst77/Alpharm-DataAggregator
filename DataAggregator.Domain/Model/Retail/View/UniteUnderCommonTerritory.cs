using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.Retail.View
{
    public class UniteUnderCommonTerritoryView
    {
        [Key]
        [Column(Order = 1)]
        public string Code { get; set; }

        [Key]
        [Column(Order = 2)]
        public int Year { get; set; }

        public string FullName { get; set; }

        public int Use { get; set; }
    }
}