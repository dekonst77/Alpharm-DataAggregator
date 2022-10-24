using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("DrugRaw", Schema = "Systematization")]
    public class DrugRaw
    {
        public long Id { get; set; }

        public long? DrugClearId { get; set; }

        public string Text { get; set; }

        public string Manufacturer { get; set; }

        public long SourceId { get; set; }

        public virtual DrugClear DrugClear { get; set; }

        public virtual Source Source { get; set; }
    }
}
