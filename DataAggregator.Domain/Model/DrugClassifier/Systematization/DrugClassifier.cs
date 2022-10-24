using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;


namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("DrugClassifier", Schema = "Systematization")]
    public class DrugClassifier
    {
        public long Id { get; set; }

        public long DrugClearPeriodId { get; set; }

        public long? DrugId { get; set; }

        public long? OwnerTradeMarkId { get; set; }

        public long? PackerId { get; set; }

        public int? ConsumerPackingCount { get; set; }
      
        public long? RobotId { get; set; }

        public int? RobotVersion { get; set; }

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
        public Int16 type { get; set; }

        /// <summary>
        /// Ошибочное действие предыдущего пользователя
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Пользователь, отправивший данные на проверку
        /// </summary>
        public Guid? ForCheckingUserId { get; set; }

        /// <summary>
        /// Пользователь, отправивший данные на заведение
        /// </summary>
        public Guid? ForAddingUserId { get; set; }

        /// <summary>
        /// Пользователь, пометивший данные как прочие
        /// </summary>
        public Guid? IsOtherUserId { get; set; }

        /// <summary>
        /// Пользователь, ошибочно отправивший данные на заведение
        /// </summary>
        public Guid? ForAddingErrorUserId { get; set; }

        /// <summary>
        /// Пользователь, ошибочно пометивший данные как прочие
        /// </summary>
        public Guid? IsOtherErrorUserId { get; set; }

        /// <summary>
        /// Пользователь, сделавший последнюю привязку данных
        /// </summary>
        public Guid? LastChangedUserId { get; set; }

        public virtual DrugClearPeriod DrugClearPeriod { get; set; }

        public virtual Drug Drug { get; set; }

        public virtual Manufacturer OwnerTradeMark { get; set; }

        public virtual Robot Robot { get; set; }
    }
}
