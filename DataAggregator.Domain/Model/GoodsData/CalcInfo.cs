using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GoodsData
{
    [Table("CalcInfo", Schema = "calc")]
    public class CalcInfo
    {
        [Key, Column(Order = 0)]
        public int Year { get; set; }
        [Key, Column(Order = 1)]
        public int Month { get; set; }
        public DateTime? DateCalc { get; set; }
        public DateTime? DateCopy { get; set; }
        public DateTime? DateExecPriceRules { get; set; }
        public DateTime? DateExecCountRules { get; set; }
        public DateTime? DateUpdatePriceRules { get; set; }
        public DateTime? DateUpdateCountRules { get; set; }
        public DateTime? DateExport { get; set; }
    }
}