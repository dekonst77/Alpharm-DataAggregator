using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class DrugClassification
    {
        [Key, Column(Order = 0)]
        public long DrugId { get; set; }
        [Key, Column(Order = 1)]
        public long OwnerTradeMarkId { get; set; }
        public long? ATCWhoId { get; set; }
        public long? ATCEphmraId { get; set; }
        public long? FTGId { get; set; }
        public bool IsOtc { get; set; }
        public bool IsRx { get; set; }
        public bool IsExchangeable { get; set; }
        public bool IsReference { get; set; }
        public long? ATCBaaId { get; set; }
        public long? NFCId { get; set; }
        public long? BrandId { get; set; }
        public DateTime? FilledParametersDate { get; set; }
        public DateTime? LastChangedParametersDate { get; set; }
        public Guid? LastChangedParametersUserId { get; set; }

        public bool DDD_chek { get; set; }
        public System.Single? DDD_Norma { get; set; }
        public string DDD_Units { get; set; }
        public string DDD_Comment { get; set; }
        public string DDD_Formula { get; set; }
        public decimal? DDDs { get; set; }
        public virtual NFC Nfc { get; set; }
        public virtual ATCWho ATCWho { get; set; }
        public virtual ATCEphmra ATCEphmra { get; set; }
        public virtual ATCBaa ATCBaa { get; set; }
        public virtual FTG FTG { get; set; }
        public virtual Brand Brand { get; set; }

        [ForeignKey("DDD_Units")]
        public virtual DDD_Units DDD_Unitss { get; set; }

        public DrugClassification Copy()
        {
            DrugClassification drugClassification = new DrugClassification();
            drugClassification.ATCWhoId = this.ATCWhoId;
            drugClassification.ATCEphmraId = this.ATCEphmraId;
            drugClassification.FTGId = this.FTGId;
            drugClassification.IsOtc = this.IsOtc;
            drugClassification.IsRx = this.IsRx;
            drugClassification.IsExchangeable = this.IsExchangeable;
            drugClassification.IsReference = this.IsReference;
            drugClassification.ATCBaaId = this.ATCBaaId;
            drugClassification.NFCId = this.NFCId;
            drugClassification.BrandId = this.BrandId;
            drugClassification.FilledParametersDate = this.FilledParametersDate;
            drugClassification.LastChangedParametersDate = this.LastChangedParametersDate;
            drugClassification.LastChangedParametersUserId = this.LastChangedParametersUserId;

            return drugClassification;
        }
        public DrugClassification()
        {
            ATCWhoId = 11090;
            ATCEphmraId = 1247;
            FTGId = 631;
            IsOtc = false;
            IsRx = false;
            IsExchangeable = false;
            IsReference = false;
            ATCBaaId = 85;
            NFCId = 694;
            BrandId = 18727;
            DDD_Comment = "";
            DDD_Units = "";
            DDD_Norma = 0;
            DDD_chek = false;
        }
        public void IsNull()
        {
            if (DDD_Comment == null) DDD_Comment = "";
            if (DDD_Units == null) DDD_Units = "";
            //if (DDD_Norma == null) DDD_Norma = 0;
            //if (DDD_chek == null) DDD_chek = false;
        }
    }
}