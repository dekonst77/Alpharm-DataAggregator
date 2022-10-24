using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DataAggregator.Domain.BulkInsert
{
    public static class Extensions
    {
        public static GenericListDataReader<T> GetDataReader<T>(this IEnumerable<T> list)
        {
            return new GenericListDataReader<T>(list);
        }

        public static void BulkInsert<T>(this DbContext context, List<T> data) where T : class
        {
           var insert = new DataAggregator.Domain.BulkInsert.BulkInsert();
           insert.Insert(context, data);
        }

        public static string GetTableName<T>(this DbContext context) where T : class
        {

            var attribute = typeof(T).GetCustomAttributes(typeof (TableAttribute), true).FirstOrDefault() as TableAttribute;

            if(attribute == null)
                throw  new ApplicationException("Атрибут схемы и имени таблицы не найден");

            return string.Format("{0}.{1}", attribute.Schema, attribute.Name);
        }
    }
}
