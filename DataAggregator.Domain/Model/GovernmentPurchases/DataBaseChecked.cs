using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("DataBaseChecked", Schema = "logs")]
    public class DataBaseChecked
    {
        [Key]
        public string Database { get; set; }
        public bool Checked { get; set; }
        public DateTime? CheckedDate { get; set; }
        public bool Created { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
