using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DataLoader
{
    public class TaskParameter
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
        /// Ключ параметра
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Значение параметра
        /// </summary>
        public string Value { get; set; }

        public Task Task { get; set; } 
    }
}
