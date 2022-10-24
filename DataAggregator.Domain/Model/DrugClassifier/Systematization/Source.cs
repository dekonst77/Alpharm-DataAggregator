using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DrugClassifier.Systematization
{
    [Table("Source", Schema = "Systematization")]
    public class Source
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
            public virtual IList<DrugClear> DrugClear { get; set; }
        [JsonIgnore]
        public virtual IList<DrugRaw> DrugRaw { get; set; }
        [JsonIgnore]
        public virtual IList<Period> Period { get; set; } 
    }

    [Table("PrioritetWords", Schema = "Systematization")]
    public class PrioritetWords
    {
        [Key]
        public int Id { get; set; }

        public long SourceId { get; set; }

        public string Value { get; set; }
        public string Name { get; set; }
    }

    [Table("PrioritetDrugClassifier", Schema = "Systematization")]
    public class PrioritetDrugClassifier
    {
        [Key]
        [Column(Order = 1)]
        public long DrugClassifierId { get; set; }
        [Key]
        [Column(Order = 10)]
        public int PrioritetWordsId { get; set; }

        public bool isControl { get; set; }
    }
}
