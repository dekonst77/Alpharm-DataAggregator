using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.Report
{
    [Table("Status", Schema = "report")]
    public class Status
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
