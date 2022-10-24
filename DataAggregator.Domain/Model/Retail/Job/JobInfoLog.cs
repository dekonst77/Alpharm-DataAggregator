using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail.SendToClassification
{
    [Table("Log", Schema = "Job")]
    public class JobInfoLog
    {
        public long Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
        public string Step { get; set; }
        public string JobName { get; set; }
    }
}
