using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DataLoader
{
    /// <summary>
    /// Загруженные данные
    /// </summary>
    public class RawData
    {
        /// <summary>
        /// GUID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Ссылка на страницу с которой загружены данные
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Содержимое страницы
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Версия DataLoader, с помощью которой была произведена загрузка даннных
        /// </summary>
        public string DataLoaderVersion { get; set; }

        /// <summary>
        /// Дата загрузки
        /// </summary>
        public DateTime DateLoad { get; set; }

        /// <summary>
        /// Id Task-а согласно которому была произведена загрузка
        /// </summary>
        public long TaskId { get; set; }

        /// <summary>
        /// Id который подставляется в маску при загрузке по списку url
        /// </summary>
        public long? DataId { get; set; }

        public Task Task { get; set; }
    }
}
