using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsSystematization.View
{
    [Table("CategoryStatView", Schema = "GoodsSystematization")]
    public class CategoryStatView
    {
        [Key]
        public Guid Id { get; set; }

        public long? CategoryId { get; set; }

        public bool ForAdding { get; set; }

        public string CategoryName { get; set; }

        public int ForWorkCount { get; set; }
        public int InWorkCount { get; set; }
        public int IsReadyCount { get; set; }
    }
}