using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DataLoader
{
    public class LoadStatus
    {
        public LoadStatus()
        {
            TaskLoadStatuses = new List<TaskLoadStatus>();
        }

        /// <summary>
        /// Идентификатор статуса
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Название статуса
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        public int Number { get; set; }

        public virtual ICollection<TaskLoadStatus> TaskLoadStatuses { get; set; } 
    }
}
