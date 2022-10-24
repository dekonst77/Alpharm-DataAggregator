using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("Period", Schema = "Systematization")]
    public class Period
    {
        public long Id { get; set; }

        public int? YearStart { get; set; }

        public int? MonthStart { get; set; }

        public int? YearEnd { get; set; }

        public int? MonthEnd { get; set; }

        public string Name { get; set; }

        public virtual long SourceId { get; set; }

        [JsonIgnore]
        public virtual Source Source { get; set; }

        public virtual IList<DrugClearPeriod> DrugClearPeriod { get; set; } 
    }
}
