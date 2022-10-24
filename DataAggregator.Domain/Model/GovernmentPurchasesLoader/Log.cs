using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("Log", Schema = "dbo")]
    public class Log
    {
        public long Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string Comment { get; set; }
    }
}