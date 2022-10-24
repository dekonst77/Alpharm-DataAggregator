using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Models.Classifier
{
    public class InnGroupJson
    {
        public long Id { get; set; }
        public string Value { get; set; }

        public InnGroupJson(INNGroup innGroup)
        {
            Id = innGroup.Id;
            Value = innGroup.Description;
        }

    }
}