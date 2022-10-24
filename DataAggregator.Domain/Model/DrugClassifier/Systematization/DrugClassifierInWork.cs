using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("DrugClassifierInWork", Schema = "Systematization")]
    public class DrugClassifierInWork
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }

        public long DrugClearPeriodId { get; set; }

        public long? DrugId { get; set; }

        public long? OwnerTradeMarkId { get; set; }

        public long? PackerId { get; set; }

        public int? ConsumerPackingCount { get; set; }
        
        /// <summary>
        /// Количество пользовательских упаковок в коробке
        /// </summary>
        public int? RealPackingCount { get; set; }

        /// <summary>
        /// Данные на проверку
        /// </summary>
        public bool ForChecking { get; set; }

        /// <summary>
        /// Данные на заведение
        /// </summary>
        public bool ForAdding { get; set; }

        /// <summary>
        /// Прочие данные
        /// </summary>
        public bool IsOther { get; set; }

        public bool SuperCheck { get; set; }
        //public Int16 type { get; set; }
        // public string GoodsCategoryName { get; set; }
        public long? GoodsId { get; set; }
        public byte? GoodsCategoryId { get; set; }
        /// <summary>
        /// Ошибочное действие предыдущего пользователя
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Были изменения в текущем цикле работы (с момента взятия данных пользователем).
        /// </summary>
        public bool? HasChanges { get; set; }

        /// <summary>
        /// Акция (2+1 и т.п.)
        /// </summary>
        public string Promo { get; set; }
        public string Flags { get; set; }
        public string Comment { get; set; }

        public virtual DrugClearPeriod DrugClearPeriod { get; set; }

        public virtual Drug Drug { get; set; }

        public virtual Manufacturer OwnerTradeMark { get; set; }

       
        public void ClearDrugId()
        {
            this.DrugId = null;
            this.GoodsId = null;
            this.OwnerTradeMarkId = null;
            this.PackerId = null;
            this.ConsumerPackingCount = null;
            this.RealPackingCount = null;
        }
    }
}
