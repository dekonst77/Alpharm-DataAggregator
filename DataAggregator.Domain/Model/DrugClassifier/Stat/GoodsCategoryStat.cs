using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Stat
{
    [Table("GoodsCategoryStat", Schema = "Stat")]
    public class GoodsCategoryStat
    {
        public long Id { get; set; }
        public long? CategoryId { get; set; }
        public bool ForAdding { get; set; }
        public string CategoryName { get; set; }
        public int ForWorkCount { get; set; }
        public int InWorkCount { get; set; }
        public int IsReadyCount { get; set; }

        [JsonIgnore]
        [ForeignKey("CategoryId")]
        public virtual GoodsCategory GoodsCategory { get; set; }
    }

    [Table("PrioritetStat", Schema = "Stat")]
    public class PrioritetStat
    {
        [Key]
        [Column(Order = 1)]
        public long PeriodId { get; set; }
        [Key]
        [Column(Order = 2)]
        public bool isReady { get; set; }
        [Key]
        [Column(Order = 3)]
        public bool IsOther { get; set; }
        [Key]
        [Column(Order = 4)]
        public int PrioritetWordsId { get; set; }
        public string RuleName { get; set; }
        public long Count { get; set; }
        [NotMapped]
        public bool IsChecked { get; set; }
    }

    [Table("CategoryStatDrugView", Schema = "Stat")]
    public class CategoryStatDrugView
    {
        [Key]
        public long Id { get; set; }
        public long? PeriodId { get; set; }
        public long? SourceId { get; set; }
        public long? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SectionName { get; set; }
        public int ToWorkCount { get; set; }
        public int ToAddingCount { get; set; }
        public int ToCheckingCount { get; set; }
        public int IsReadyCount { get; set; }
        [NotMapped]
        public bool IsChecked { get; set; }
    }
}