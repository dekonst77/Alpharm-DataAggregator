using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("GoodsClassifierInWork", Schema = "GoodsSystematization")]
    public class GoodsClassifierInWork
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public long GoodsClearId { get; set; }

        public long? GoodsId { get; set; }

        public long? OwnerTradeMarkId { get; set; }

        public long? PackerId { get; set; }

        /// <summary>
        /// Данные на заведение
        /// </summary>
        public bool ForAdding { get; set; }

        /// <summary>
        /// Были изменения в текущем цикле работы (с момента взятия данных пользователем).
        /// </summary>
        public bool? HasChanges { get; set; }

        public long? GoodsCategoryId { get; set; }

        public void ClearGoodsId()
        {
            GoodsId = null;
            OwnerTradeMarkId = null;
            PackerId = null;
            GoodsCategoryId = null;
        }

        public void ClearFlags()
        {
            ForAdding = false;
        }
    }
}
