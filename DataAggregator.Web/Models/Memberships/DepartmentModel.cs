namespace DataAggregator.Web.Models.Memberships
{
    public sealed class DepartmentModel
    {
        /// <summary>
        /// Идентификатор подразделения
        /// </summary>
        public long? Id { get; set; }

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
        public string ManagerName { get; set; }
    }
}