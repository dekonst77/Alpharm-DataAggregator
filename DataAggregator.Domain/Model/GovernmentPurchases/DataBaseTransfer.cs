using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("DataBaseTransfer", Schema = "logs")]
    public class DataBaseTransfer
    {
        public long Id { get; set; }
        [MaxLength(50)]
        public string Step { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
    }
}
