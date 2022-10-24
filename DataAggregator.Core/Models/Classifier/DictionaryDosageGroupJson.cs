using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Models.Classifier
{
    public class DictionaryDosageGroupJson : DosageGroup
    {
        public string Value { get; set; }

        public DictionaryDosageGroupJson(DosageGroup dosageGroup)
        {
            Id = dosageGroup.Id;
            Value = dosageGroup.Description;
        }
    }
}