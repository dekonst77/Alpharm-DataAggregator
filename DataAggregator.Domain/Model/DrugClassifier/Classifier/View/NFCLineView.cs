using DataAggregator.Domain.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class NFCLineView
    {
        public long Nfc1Id { get; set; }
        public string Nfc1Value { get; set; }
        public string Nfc1Description { get; set; }

        public long Nfc2Id { get; set; }
        public string Nfc2Value { get; set; }
        public string Nfc2Description { get; set; }

        [Key]
        public string Nfc3Value { get; set; }
        public long? Nfc3Id { get; set; }
        public string Nfc3Description { get; set; }

        public System.Int16 RouteAdministrationId { get; set; }
    }
}