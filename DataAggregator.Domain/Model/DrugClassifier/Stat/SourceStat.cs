using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("SourceStat", Schema = "Stat")]
    public class SourceStat
    {
        [Key, Column(Order = 1)]
        public long PeriodId { get; set; }
        [Key, Column(Order = 2)]
        public long SourceId { get; set; }

        public long? ForCheckingCount { get; set; }

        public long? ForAddingCount { get; set; }

        public long? WorkCount { get; set; }
        public long? WorkCount_Dop { get; set; }
    }
}
