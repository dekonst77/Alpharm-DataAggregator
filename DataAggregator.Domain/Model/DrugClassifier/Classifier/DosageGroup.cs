using System.Collections.Generic;
using DataAggregator.Domain.Model.DrugClassifier.SearchTerms;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{

    public class DosageGroup
    {
        public long Id { get; set; }

        //Объем дозировки
        public string DosageValueCount { get; set; }

        public long? DosageValueId { get; set; }

        public virtual Dosage DosageValue { get; set; }

        //Общий объем дозировки
        public string TotalVolumeCount { get; set; }
        public long? TotalVolumeId { get; set; }

        public virtual Dosage TotalVolume { get; set; }

        /// <summary>
        /// Описание дозировки текстом
        /// </summary>
        public string Description { get; set; }

        public string Description_Eng { get; set; }

        /// <summary>
        /// Короткое описание дозировки текстом без МНН
        /// </summary>
        public string ShortDescription { get; set; }

        [JsonIgnore]
        public virtual ICollection<INNDosage> INNDosages { get; set; }

        [JsonIgnore]
        public virtual ICollection<Drug> Drugs { get; set; }

        [JsonIgnore]
        public virtual ICollection<SynDosageGroup> SynDosageGroups { get; set; }

    }
}
