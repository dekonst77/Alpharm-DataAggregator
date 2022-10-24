using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.FtpPoisklekarstv
{
    public class File
    {
        public long Id { get; set; }

        /// <summary>
        /// Имя файла архива
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дата обновления на FTP
        /// </summary>
        public DateTime? LastUpdate { get; set; }

        /// <summary>
        /// Дата загрузки с FTP
        /// </summary>
        public DateTime? LastLoad { get; set; }

        /// <summary>
        /// Дата когда последний раз проверяли файл на необходимость загрузки
        /// </summary>
        public DateTime? LastTryLoad { get; set; }

        /// <summary>
        /// Последнее расположение архива, из которого было успешно загружено
        /// </summary>
        public string LastSuccessFolder { get; set; }

        /// <summary>
        /// Признак что загрузка остановлена из-за ошибки
        /// </summary>
        public bool HasErrors { get; set; }

        /// <summary>
        /// Последнее сообщение об ошибке
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Приоритет загрузки на SqlServer
        /// </summary>
        public int Priority { get; set; }
    }
}
