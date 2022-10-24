using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    [Table("ProductionInfo", Schema = "Classifier")]
    public class ProductionInfo
    {
        public long Id { get; set; }

        public long OwnerTradeMarkId { get; set; }

        public long PackerId { get; set; }

        public long DrugId { get; set; }

        public long? RegistrationCertificateId { get; set; }

        public virtual RegistrationCertificate RegistrationCertificate { get; set; }

        public virtual Manufacturer OwnerTradeMark { get; set; }

        public virtual Manufacturer Packer { get; set; }

        public virtual Drug Drug { get; set; }

        public bool WithoutRegistrationCertificate { get; set; }

        public bool Used { get; set; }
        public string Comment { get; set; }

        public string Comment2 { get; set; }
        public DateTime? Data_setBlock { get; set; }
        public DateTime? Data_Block { get; set; }
        public DateTime? Data_UnBlock { get; set; }

        public decimal kofPriceGZotkl { get; set; }

        public decimal? PriceVED { get; set; }

        public decimal? PriceEtalon { get; set; }

        public long? ProductionStageId { get; set; }        
        public virtual  ProductionStage ProductionStage { get; set; }
        
        public long? ProductionLocalizationId { get; set; }
        public virtual Localization ProductionLocalization { get; set; }

        public ProductionInfo Copy()
        {
            return new ProductionInfo
            {
                OwnerTradeMarkId = this.OwnerTradeMarkId,
                PackerId = this.PackerId,
                DrugId = this.DrugId,
                Drug = this.Drug,
                OwnerTradeMark =  this.OwnerTradeMark,
                RegistrationCertificate = this.RegistrationCertificate,
                RegistrationCertificateId = this.RegistrationCertificateId,
                Id = this.Id                
            };
        }
    }


    public class ProductionInfoView
    {
        public long Id { get; set; }
        public long? ClassifierId { get; set; }

        public long OwnerTradeMarkId { get; set; }

        public long PackerId { get; set; }

        public long DrugId { get; set; }

        public long? RegistrationCertificateId { get; set; }
        public bool WithoutRegistrationCertificate { get; set; }

        public bool Used { get; set; }
        public string Comment { get; set; }
        public string Comment2 { get; set; }
        public DateTime? Data_setBlock { get; set; }
        public DateTime? Data_Block { get; set; }
        public DateTime? Data_UnBlock { get; set; }

        public decimal kofPriceGZotkl { get; set; }
        public string TradeName { get; set; }
        public string OwnerTradeMark { get; set; }
        public string Packer { get; set; }
        public string FV { get; set; }
    }
}