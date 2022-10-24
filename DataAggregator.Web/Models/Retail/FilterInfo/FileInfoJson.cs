using System;
using System.Collections.Generic;

namespace DataAggregator.Web.Models.Retail.FilterInfo
{
    public class Dictionary
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Списки фильтров для поиска
    /// </summary>
    public class FileInfoJson
    {
        /// <summary>
        /// Список источников
        /// </summary>
        public List<Domain.Model.Retail.Source> SourceList { get; set; }

        /// <summary>
        /// Выбранный фильтр
        /// </summary>
        public SelectFilter Filter { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public FileInfoJson()
        {
            Filter = new SelectFilter();

            var date = DateTime.Now.AddMonths(-1);
            Filter.Year = date.Year;
            Filter.Month = date.Month;
       
            Filter.Source = null;
        }
    }
}