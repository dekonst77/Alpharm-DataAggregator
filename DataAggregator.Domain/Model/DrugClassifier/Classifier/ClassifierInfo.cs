using DataAggregator.Domain.Model.DrugClassifier.GoodsClassifier;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class ClassifierInfo
    {
        [Key]
        public long Id { get; set; }
        public long? ProductionInfoId { get; set; }     
        public long? GoodsProductionInfoId { get; set; }

        public virtual ProductionInfo ProductionInfo { get; set; }    
        public virtual GoodsProductionInfo GoodsProductionInfo { get; set; }
        public virtual ICollection<ClassifierPacking> ClassifierPackings { get; set; }

        public bool? ToRetail { get; set; }
        public bool? ToOFD { get; set; }
        public string ci_comment { get; set; }
        public bool ToBlockUsed { get; set; }
        public bool ToSplitMnn { get; set; }
        public bool IsSTM { get; set; }
    }

    public class ClassifierInfo_Report
    {
        [Key]
        public long Id { get; set; }
        
        public decimal OFD_Sum_LastMonth { get; set; }
        public decimal Audit_Sum_LastMonth { get; set; }

        public bool ToRetail { get; set; }
        public bool ToOFD { get; set; }
        public string ci_comment { get; set; }
        public string TradeName { get; set; }
        public string INNGroup { get; set; }
        public string DrugDescription { get; set; }
        public int DrugId { get; set; }
        public int OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public bool Used { get; set; }
        public bool IsOther { get; set; }

        public string RegistrationCertificateNumber { get; set; }
        public string Packer { get; set; }

        public DateTime? LastWhen { get; set; }
        public decimal? PriceNew { get; set; }

        public bool ToBlockUsed { get; set; }
        public bool ToSplitMnn { get; set; }
        public bool IsSTM { get; set; }

        public string OperatorComments { get; set; }
    }
}
