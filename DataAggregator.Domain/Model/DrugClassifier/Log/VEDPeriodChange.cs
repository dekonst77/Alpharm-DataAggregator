using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Log
{
    [Table("VEDPeriodChange", Schema = "Log")]
    public class VEDPeriodChange
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long VedPeriodId { get; set; }
        public int? YearStart { get; set; }
        public int? MonthStart { get; set; }
        public int? YearEnd { get; set; }
        public int? MonthEnd { get; set; }
        public Guid UserId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Date { get; set; }
        public long ActionTypeId { get; set; }

        public virtual ActionType ActionType { get; set; }
    }
}
