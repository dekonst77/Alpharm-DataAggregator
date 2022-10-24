using System.Collections.Generic;

namespace DataAggregator.Domain.Model.DrugClassifier.SearchTerms
{
    public abstract class AbstractSynonym
    {
        public long Id { get; set; }

        /// <summary>
        /// Ссылка на оригинальное значение справочника
        /// </summary>
        public long OriginalId { get; set; }
        /// <summary>
        /// Значение-синоним
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Данный синоним 100% верный
        /// </summary>
        public bool IsForced { get; set; }

        /// <summary>
        /// Количество добавленй данного синонима
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Синоним копия справочного значений
        /// </summary>
        public bool IsDictionary { get; set; }

        //public virtual IList<Systematization.DrugClassifier> DrugClassifier { get; set; }

        //public virtual IList<Systematization.DrugClassifierInWork> DrugClassifierInWork { get; set; }

        
    }
}
