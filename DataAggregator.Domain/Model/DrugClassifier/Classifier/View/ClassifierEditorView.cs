using System;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class ClassifierEditorView 
    {
        [Key]
        public Guid Guid { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public long? DrugId { get; set; }
        public long? TradeNameId { get; set; }
        public string TradeName { get; set; }
        public long LKCU { get; set; }
        public string DrugDescription { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMarkKey { get; set; }
        public string OwnerTradeMark { get; set; }
        public long? DosageGroupId { get; set; }
        public string DosageGroup { get; set; }
        public long? INNGroupId { get; set; }
        public string INNGroup { get; set; }
        public long? FormProductId { get; set; }
        public string FormProduct { get; set; }
        public int ConsumerPackingCount { get; set; }
        public long? PackerId { get; set; }
        public string PackerKey { get; set; }
        public string Packer { get; set; }
        public bool Used { get; set; }
    }
}