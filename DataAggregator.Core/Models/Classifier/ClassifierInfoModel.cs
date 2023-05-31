using System;
using System.Collections.Generic;
using DataAggregator.Domain.Model.DrugClassifier.Classifier;
using Newtonsoft.Json;

namespace DataAggregator.Core.Models.Classifier
{
    public class ClassifierInfoModel
    {
        public bool OwnerTradeMarkIdNew { get; set; }
        
        public bool PackerIdNew { get; set; }

        public long OwnerRegistrationCertificateId { get; set; }

        public bool OwnerRegistrationCertificateIdNew { get; set; }
        
        public List<string> KCU { get; set; }

        public long? DrugId { get; set; }
        public bool IsDrugNew { get; set; } // новый товар

        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }

        public string RegistrationCertificateNumber { get; set; }
        public bool RegistrationCertificateNumberNew { get; set; }
        public bool? RegistrationCertificateIsBlockedOldValue { get; set; }
        public bool Used { get; set; }
        public string Comment { get; set; }

        [JsonIgnore]
        public ProductionInfo ProductionInfo { get; set; }

        [JsonIgnore]
        public Drug Drug { get; set; }

        public string FullDrugDescription
        {
            get
            {
                if(this.Drug == null)
                    return  String.Empty;
                
                return string.Format("{0} {1} {2} {3}",
                    this.Drug.TradeName != null ? this.Drug.TradeName.Value:string.Empty,
                    this.Drug.FormProduct != null ? this.Drug.FormProduct.Value : string.Empty,
                    this.Drug.DosageGroup != null ? this.Drug.DosageGroup.Description : string.Empty,
                    this.Drug.ConsumerPackingCount);

            }
        }
    }

  
}