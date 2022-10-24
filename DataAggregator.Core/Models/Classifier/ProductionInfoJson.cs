using DataAggregator.Domain.Model.DrugClassifier.Classifier;

namespace DataAggregator.Core.Models.Classifier
{
    public class ProductionInfoJson
    {
        public long Id { get; set; }
        public DictionaryJson OwnerTradeMark { get; set; }
        public DictionaryJson Packer { get; set; }
        public DictionaryJson OwnerRegistrationCertificate { get; set; }

        public ProductionInfoJson()
        {
        }

        public ProductionInfoJson(ProductionInfo productionInfo)
        {
            OwnerTradeMark = new DictionaryJson(productionInfo.OwnerTradeMark);
            Packer = new DictionaryJson(productionInfo.Packer);
            Id = productionInfo.Id;
        }
    }
}