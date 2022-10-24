using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace DataAggregator.Domain.Model.Retail.history
{
    [Table("CountRuleUsedView", Schema = "history")]
    public class CountRuleUsed
    {
        [Key, Column(Order = 0)]
        public long CountRuleId { get; set; }
        [Key, Column(Order = 1)]
        public int Year { get; set; }
        [Key, Column(Order = 2)]
        public int Month { get; set; }
        //использована в закупках
        public bool InUsed { get; set; }
        //использована в продажах
        public bool OutUsed { get; set; }
    }
}
