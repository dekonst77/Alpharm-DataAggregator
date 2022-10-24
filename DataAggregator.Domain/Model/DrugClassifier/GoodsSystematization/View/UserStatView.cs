using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.GoodsSystematization.View
{
    [Table("UserStatView", Schema = "GoodsSystematization")]
    public class UserStatView
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int InWorkCount { get; set; }
        public int IsReadyCount { get; set; }
    }
}