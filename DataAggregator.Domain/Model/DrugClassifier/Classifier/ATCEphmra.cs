using DataAggregator.Domain.Model.Common;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class ATCEphmra : HierarchicalDictionaryItem<ATCEphmra>
    {
        public bool IsUse { get; set; }
    }
}