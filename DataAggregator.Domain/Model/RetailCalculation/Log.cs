using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace DataAggregator.Domain.Model.RetailCalculation
{
    [Table("Log", Schema = "calculation")]
    public class Log
    {
        public Int64 Id { get; set; }
        public string Step { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime Date { get; set; }
    }
}
