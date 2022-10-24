using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("ClassifierPacking", Schema = "Classifier")]
    public class ClassifierPacking
    {
        [Key]
        public int Id { get; set; }
        
        public long ClassifierId { get; set; }
        [JsonIgnore,ForeignKey("ClassifierId")]
        public virtual ClassifierInfo CI { get; set; }

        public long? PrimaryPackingId { get; set; }
        public virtual Packing PrimaryPacking { get; set; }
        public int CountInPrimaryPacking { get; set; }

        public long? ConsumerPackingId { get; set; }
        public virtual Packing ConsumerPacking { get; set; }
        public int CountPrimaryPacking { get; set; }

        public string PackingDescription { get; set; }
    }
}