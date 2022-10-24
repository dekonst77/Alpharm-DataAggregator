using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Web
{
    public class Department
    {
        /// <summary>
        /// Идентификатор подразделения
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Сокращённое наименование подразделения
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Идентификатор руководителя подразделения
        /// </summary>
        public string ManagerId { get; set; }

        /// <summary>
        /// Руководитель подразделения
        /// </summary>
        public virtual ApplicationUser Manager { get; set; }
    }

}