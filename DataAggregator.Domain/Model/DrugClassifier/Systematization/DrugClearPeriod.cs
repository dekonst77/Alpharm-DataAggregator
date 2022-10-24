using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAggregator.Domain.DAL;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("DrugClearPeriod", Schema = "Systematization")]
    public class DrugClearPeriod
    {
        public long Id { get; set; }

        public long DrugClearId { get; set; }

        public long PeriodId { get; set; }

        public virtual DrugClear DrugClear { get; set; }

        public virtual Period Period { get; set; }

        //public virtual IList<StatusHistory> StatusHistory { get; set; }

        public virtual IList<DrugClassifier> DrugClassifier { get; set; }

        public virtual IList<DrugClassifierInWork> DrugClassifierInWork { get; set; }
    }
}
