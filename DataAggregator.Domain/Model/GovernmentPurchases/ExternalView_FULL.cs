using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("ExternalView_FULL", Schema = "dbo")]
    public class ExternalView_FULL
    {
        [Key]
        public long ClassifierId { get; set; }
        public bool IsOther { get; set; }
        public int DrugId { get; set; }
        public int GoodsId { get; set; }
        public string TradeName { get; set; }
        public string INNGroup { get; set; }
        public string DosageGroup { get; set; }
        public string FormProduct { get; set; }
        public int? ConsumerPackingCount { get; set; }
        public int OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public bool IsBad { get; set; }
        public string FTG { get; set; }
        public string ATCWhoCode { get; set; }
        public string ATCWhoDescription { get; set; }
        public string ATCEphmraCode { get; set; }
        public string ATCEphmraDescription { get; set; }
        public string ATCBaaCode { get; set; }
        public string ATCBaaDescription { get; set; }
        public string NFCCode { get; set; }
        public string NFCDescription { get; set; }
        public bool IsOtc { get; set; }
        public bool IsRx { get; set; }
        public int PackerId { get; set; }
        public string Packer { get; set; }
        public string DrugDescription { get; set; }
        public string Brand { get; set; }
        public long? OwnerRegistrationCertificateId { get; set; }
        public string OwnerRegistrationCertificate { get; set; }
        public int CorporationId { get; set; }
        public string Corporation { get; set; }
        public short CountryId { get; set; }
        public string Country { get; set; }
        public short LocalizationId { get; set; }
        public string Localization { get; set; }
        public string TotalVolumeCount { get; set; }
        public long? TotalVolumeId { get; set; }
        public string TotalVolume { get; set; }
        public bool Exchangeable { get; set; }
        public bool Reference { get; set; }
        public bool? WithoutRegistrationCertificate { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public decimal? kofPriceGZotkl { get; set; }
        public bool? Used { get; set; }
        public string GoodsCategoryName { get; set; }
        public string SectionName { get; set; }
        public long? INNGroupId { get; set; }
        public int TradeNameId { get; set; }
        public int BrandId { get; set; }
        public long? DosageGroupId { get; set; }
        public long? FormProductId { get; set; }
        public long? FTGId { get; set; }
        public long? ATCWhoId { get; set; }
        public long? ATCEphmraId { get; set; }
        public long? ATCBaaId { get; set; }
        public long? NFCId { get; set; }
        public bool? IsBlocked { get; set; }
        public DateTime? Data_Block { get; set; }
        public DateTime? Data_UnBlock { get; set; }
        public string Comment2 { get; set; }
        public long? ProductionInfoId { get; set; }
        public decimal? Price { get; set; }
        public long? EquipmentId { get; set; }
        public string Equipment { get; set; }
        public bool? IsCompoundBAA { get; set; }
        public string NFCDescription_Eng { get; set; }
        public string ATCWhoDescription_Eng { get; set; }
        public string FTG_Eng { get; set; }
        public string ATCEphmraDescription_Eng { get; set; }
        public string ATCBaaDescription_Eng { get; set; }
        public short? GoodsCategoryId { get; set; }
        public string TradeName_Eng { get; set; }
        public string Brand_Eng { get; set; }
        public string OwnerTradeMark_Eng { get; set; }
        public string Corporation_Eng { get; set; }
        public string Packer_Eng { get; set; }
        public bool isCurrent { get; set; }
        public string Comment { get; set; }
        public string DrugType { get; set; }
        public System.Single DDD_Norma { get; set; }
        public string DDD_Units { get; set; }
        public decimal DDDs { get; set; }
        public string FormProduct_Eng { get; set; }
        public string DrugDescription_Eng { get; set; }
        public string INNGroup_Eng { get; set; }
        public string DosageGroup_Eng { get; set; }
        public string TotalVolume_Eng { get; set; }
        public short EIId { get; set; }
        public string EI { get; set; }
        public string EI_Eng { get; set; }
        public decimal StandardUnits { get; set; }
        public bool IsSTM { get; set; }
        public long? ProductionStageId { get; set; }
        public string ProductionStage { get; set; }
        public string ProductionStage_Eng { get; set; }
        public string Equipment_Eng { get; set; }
    }
}
