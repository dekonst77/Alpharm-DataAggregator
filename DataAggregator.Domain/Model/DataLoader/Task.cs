using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DataLoader
{
    public class Task
    {
        public Task()
        {
            RawData = new List<RawData>();
            SubTasks = new List<Task>();
            TaskParameters = new List<TaskParameter>();
            TaskLoadStatuses = new List<TaskLoadStatus>();
        }

        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Если задача сгенерирована загрузчиком - ссыка на родителя
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// Тип задачи
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Описание задачи
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Задача должна выполняться загрузчиком (true) или является более общей, объединяющей несколько других (false)
        /// </summary>
        public bool IsExecutable { get; set; }

        public Task Parent { get; set; }

        public virtual ICollection<RawData> RawData { get; set; }

        public virtual ICollection<Task> SubTasks { get; set; }

        public virtual ICollection<TaskParameter> TaskParameters { get; set; }

        public virtual ICollection<TaskLoadStatus> TaskLoadStatuses { get; set; } 
    }
}
