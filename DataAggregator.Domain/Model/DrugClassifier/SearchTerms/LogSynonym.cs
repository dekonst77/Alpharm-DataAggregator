using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.SearchTerms
{
    /// <summary>
    /// Лог присвоения синонимов
    /// </summary>
    [Table("LogSynonym", Schema = "SearchTerms")]
    public class LogSynonym
    {
        public long Id { get; set; }

        public long DrugClearId { get; set; }

        /// <summary>
        /// Имя таблицы синонимов
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Идентификатор записи в табл. синонимов
        /// </summary>
        public long RecordId { get; set; }

        /// <summary>
        /// Идентификатор asp.net юзера, гуид
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Время добавления записи
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
    }
}
