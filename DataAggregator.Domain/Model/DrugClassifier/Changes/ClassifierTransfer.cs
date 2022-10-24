using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Changes
{
    [Table("ClassifierTransfer", Schema = "changes")]
    public class ClassifierTransfer
    {
        public long Id { get; set; }

        public long ClassifierIdFrom { get; set; }

        public long ClassifierIdTo { get; set; }

        public int? YearStart { get; set; }

        public int? MonthStart { get; set; }

        public int? YearEnd { get; set; }

        public int? MonthEnd { get; set; }
      
        public DateTime Date { get; set; }

        public Guid UserId { get; set; }
    }
}
