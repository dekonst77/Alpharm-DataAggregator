using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Log
{
    [Table("ActionLog", Schema = "Log")]
    public class ActionLog
    {
        public long Id { get; set; }
        public long ActionId { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
        public long ProductionInfoId { get; set; }
        //Поддержка IsActual осуществляется на уровне тригера
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool IsActual { get; set; }
    }
}