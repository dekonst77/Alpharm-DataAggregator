using System;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class SplitByINN_Result
    {
        public long Id { get; set; }
        public Nullable<long> ProductionInfoId { get; set; }
        public Nullable<long> ProductionInfoHistoryId { get; set; }
        public Nullable<long> GoodsProductionInfoId { get; set; }
        public Nullable<decimal> OFD_Sum_LastMonth { get; set; }
        public Nullable<decimal> Audit_Sum_LastMonth { get; set; }
        public Nullable<bool> ToRetail { get; set; }
        public Nullable<bool> ToOFD { get; set; }
        public string ci_comment { get; set; }
        public bool ToBlockUsed { get; set; }
        public bool ToSplitMnn { get; set; }
        public bool IsSTM { get; set; }
        public Nullable<bool> ForOFD { get; set; }
        public bool ToSplitMnn_Signed { get; set; }
    }
}
