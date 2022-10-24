using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization.View
{
    [Table("GoodsInWorkView", Schema = "GoodsSystematization")]
    public class GoodsInWorkView
    {
        [Key]
        public long GoodsClearId { get; set; }

        public long DrugClearId { get; set; }

        public Guid UserId { get; set; }

        public long? GoodsId { get; set; }

        public long? GoodsCategoryId { get; set; }

        public string GoodsCategoryName { get; set; }
        public string Text { get; set; }
        public string Manufacturer { get; set; }

        public string GoodsTradeName { get; set; }
        public long? GoodsTradeNameId { get; set; }

        public string GoodsDescription { get; set; }

        public long? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }

        public long? PackerId { get; set; }
         public string Packer { get; set; }

        public bool ForAdding { get; set; }
        public bool? HasChanges { get; set; }

        public Guid? LastChangedUserId { get; set; }
        public string LastChangedUserName { get; set; }
    }
}
