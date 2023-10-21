using DataAggregator.Domain.DAL;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace DataAggregator.Domain.BulkInsert
{
    public class BulkInsert
    {
        public void Insert<T>(DbContext context, List<T> data) where T : class
        {
            var columns = typeof(T).GetProperties()
                .Where(property =>   property.PropertyType.IsValueType || property.PropertyType.Name.ToLower() == "string")
                .Select(property => property.Name)
                .ToList();
            SqlConnectionStringBuilder decoder;

            ConnectionStringSettings cString = ConfigurationManager.ConnectionStrings["GovernmentPurchasesContext"];
            decoder = new SqlConnectionStringBuilder(cString.ConnectionString);

            using (IDataReader reader = data.GetDataReader())
            using (SqlConnection conn = new SqlConnection(context.Database.Connection.ConnectionString + ";Password=" + decoder.Password))
            using (SqlBulkCopy bcp = new SqlBulkCopy(conn))
            {
                conn.Open();

                //Мапим значения
                columns.ForEach(c => bcp.ColumnMappings.Add(c,c));



                bcp.DestinationTableName = context.GetTableName<T>();

                bcp.WriteToServer(reader);
            }
        }
    }
}
