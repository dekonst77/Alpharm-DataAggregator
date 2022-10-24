using System.Collections.Generic;
using System.Linq;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Models.Classifier
{
    public class DrugJson
    {
        public long Id { get; set; }
        public DictionaryJson FormProduct { get; set; }
        public DosageGroup DosageGroup { get; set; }
        public int? ConsumerPackingCount { get; set; }

        public DrugJson(Drug drug)
        {
            Id = drug.Id;
            FormProduct = new DictionaryJson(drug.FormProduct);
            DosageGroup = new DictionaryDosageGroupJson(drug.DosageGroup);
            ConsumerPackingCount = drug.ConsumerPackingCount;
        }

    }
}