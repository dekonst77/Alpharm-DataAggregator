using System;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier.ClassifierRxOtc
{
    /// <summary>
    /// результат проц-ры [Classifier].[LoadClassifierRxOtc_SP]
    /// </summary>
    public class LoadClassifierRxOtc_SP_Result
    {
        public bool Used { get; set; }
        public Nullable<long> RegistrationCertificateId { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public long DrugId { get; set; }
        public long ClassifierInfoId { get; set; }
        public Nullable<long> INNGroupId { get; set; }        
        public string INN { get; set; }
        public string TN_FP_D_F_INN { get; set; }
        public string DrugTypeValue { get; set; }
        public bool Rx { get; set; }
        public bool Otc { get; set; }
        public bool RURx { get; set; }
        public Nullable<bool> IsChecked { get; set; }
        public Nullable<bool> IsException { get; set; }
    }
}
