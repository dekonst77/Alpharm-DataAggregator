using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{/*
    [Table("StatusHistory", Schema = "Systematization")]
    public class StatusHistory
    {
        public long Id { get; set; }

        public long DrugClearId { get; set; }

        public long StatusId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }

        public bool IsActual { get; set; }

        public Guid? UserId { get; set; }

        public long? RobotId { get; set; }

        /// <summary>
        /// Признак что данные (привязка к классификатору) были изменены
        /// </summary>
        public bool IsChanged { get; set; }

        /// <summary>
        /// Признак что была исправлена ошибка предыдущего оператора обрабатывавшего данные
        /// </summary>
        public bool IsFixed { get; set; }

        public virtual Status Status { get; set; }

        public virtual DrugClear DrugClear { get; set; }

        public virtual Robot Robot { get; set; }
    }*/
}
