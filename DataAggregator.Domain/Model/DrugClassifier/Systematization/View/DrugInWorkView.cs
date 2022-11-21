using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization.View
{
    [Table("DrugInWorkView", Schema = "Systematization")]
    public class DrugInWorkView
    {
        [Key]
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public long DrugClearId { get; set; }

        public long? DrugId { get; set; }

        public long? TradeNameId { get; set; }

        public string TradeName { get; set; }

        public long? FormProductId { get; set; }

        public string FormProduct { get; set; }

        public long? OwnerTradeMarkId { get; set; }

        public string OwnerTradeMark { get; set; }

        public long? PackerId { get; set; }

        public string Packer { get; set; }

        public string DrugClearText { get; set; }

        public bool ForChecking { get; set; }

        public bool SuperCheck { get; set; }

        public bool ForAdding { get; set; }

        public bool IsOther { get; set; }
        public Int16 type { get; set; }

        public bool IsError { get; set; }

        public bool? HasChanges { get; set; }

        public string DrugDescription { get; set; }

        public string Manufacturer { get; set; }

        public long? INNGroupId { get; set; }

        public long? DosageGroupId { get; set; }

        /// <summary>
        /// Кто последний делал изменения
        /// </summary>
        public string UserName { get; set; }

        public string Promo { get; set; }
        public string Flags { get; set; }

        public string GoodsCategoryName { get; set; }
        public long? GoodsId { get; set; }
        public byte? GoodsCategoryId { get; set; }
        public  string RegNumber { get; set; }
        public string Comment { get; set; }
        public string PrioritetWords { get; set; }
        public int? PrioritetWords_isControl { get; set; }
        public string OperatorComment { get; set; }

        [NotMapped]
        public bool HasEmptyClassfierId { get; set; }
    }
}
