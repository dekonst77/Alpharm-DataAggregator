using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using DataAggregator.Domain.Model.DrugClassifier.DataAnalyzer;

namespace DataAggregator.Domain.DAL
{
    public class DataAnalyzerContext : DbContext
    {
        public DbSet<SynManufacturer> SynManufacturer { get; set; }

        public List<DrugSearchInfo> FindSynonym(string text)
        {
            var words = new SqlParameter();
            words.ParameterName = "@words";
            words.Direction = ParameterDirection.Input;
            words.SqlDbType = SqlDbType.VarChar;
            words.Value = text;

            return this.Database.SqlQuery<DrugSearchInfo>("exec [SearchTerms].[FindSynonym] @words", words).ToList();
        }

        public List<ManufacturerSearchInfo> FindManufacturer(string text)
        {
            var words = new SqlParameter();
            words.ParameterName = "@words";
            words.Direction = ParameterDirection.Input;
            words.SqlDbType = SqlDbType.VarChar;
            words.Value = text;

            return this.Database.SqlQuery<ManufacturerSearchInfo>("exec [SearchTerms].[FindManufacturer] @words", words).ToList();
        }
    }
}
