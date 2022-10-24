using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.View
{
    public class ClassifierEditorFilterView 
    {
        [Key]
        public Guid Guid { get; set; }
        public long? ClassifierId { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public long? DrugId { get; set; }
        public long? TradeNameId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public long? DosageGroupId { get; set; }
        public string DosageGroup { get; set; }
        public long? INNGroupId { get; set; }
        public string INNGroup { get; set; }
        public long? FormProductId { get; set; }
        public string FormProduct { get; set; }
        public int ConsumerPackingCount { get; set; }
        public long? PackerId { get; set; }
        public string Packer { get; set; }
        public bool Used { get; set; }
        public bool IsBlockedRC { get; set; }
        public long? CertificateId { get; set; }
        public decimal? Price { get; set; }
        public long? ProductionLocalizationId { get; set; }
        public string ProductionLocalizationValue { get; set; }
        public long? PackerLocalizationId { get; set; }
        public string PackerLocalizationValue { get; set; }
    }
    public class ClassifierEditorFilterClassifierPackingView
    {
        [Key]
        [Column(Order = 1)]
        public long ClassifierId { get; set; }
        [Key]
        [Column(Order = 2)]
        public int ClassifierPackingId { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public long? DrugId { get; set; }
        public long? TradeNameId { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public long? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public long? DosageGroupId { get; set; }
        public string DosageGroup { get; set; }
        public long? INNGroupId { get; set; }
        public string INNGroup { get; set; }
        public long? FormProductId { get; set; }
        public string FormProduct { get; set; }
        public int ConsumerPackingCount { get; set; }


        public string PrimaryPacking { get; set; }
        public int? CountInPrimaryPacking { get; set; }
        public string ConsumerPacking { get; set; }
        public int? CountPrimaryPacking { get; set; }
        public string PackingDescription { get; set; }

        public long? PackerId { get; set; }
        public string Packer { get; set; }
        public bool Used { get; set; }
        public bool IsBlockedRC { get; set; }
        public decimal? Price { get; set; }
        public int ESKLPCount { get; set; }
    }
}