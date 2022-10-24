using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    [Table("ClassifierPacking_BlisterBlock_View", Schema = "Classifier")]
    public class ClassifierPacking_BlisterBlock_View
    {
        [Key]
        public int Id { get; set; }
        public long ClassifierId { get; set; }

        [JsonProperty("PrimaryPacking.Id")]
        public Nullable<long> PrimaryPackingId { get; set; }
        [JsonProperty("PrimaryPacking.Value")]
        public string PrimaryPackingValue { get; set; }

        public Nullable<int> CountInPrimaryPacking { get; set; }

        [JsonProperty("ConsumerPacking.Id")]
        public Nullable<long> ConsumerPackingId { get; set; }
        [JsonProperty("ConsumerPacking.Value")]
        public string ConsumerPackingValue { get; set; }

        public Nullable<int> CountPrimaryPacking { get; set; }
        public string PackingDescription { get; set; }
        public Nullable<bool> IsBlisterPacking { get; set; }
        public Nullable<int> ClassifierPackingId { get; set; }
    }
}
