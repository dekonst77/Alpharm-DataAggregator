using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DataLoader
{
    public class TaskLoadStatus
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Ссылка на задачу
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// Ссылка на статус
        /// </summary>
        public long LoadStatusId { get; set; }

        /// <summary>
        /// Дата начала действия статуса
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Дата окончания действия статуса
        /// </summary>
        public DateTime? DateEnd { get; set; }

        /// <summary>
        /// Актуален ли статус
        /// </summary>
        public bool IsActual { get; set; }

        /// <summary>
        /// Дополнительная информация (например об ошибке)
        /// </summary>
        public string Info { get; set; }

        public Task Task { get; set; }

        public LoadStatus LoadStatus { get; set; }
      
    }
}
