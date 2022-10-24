using System;
using System.ComponentModel.DataAnnotations;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class CalculatedDataView
    {
        public long PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public string PurchaseUrl { get; set; }
        public string PurchaseName { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime PurchaseDateBegin { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime PurchaseDateEnd { get; set; }
        public long? PurchaseCustomerId { get; set; }
        public string PurchaseDeliveryTime { get; set; }

        public Byte MethodId { get; set; }
        public string MethodName { get; set; }

        public Byte PurchaseClassId { get; set; }
        public string PurchaseClassName { get; set; }

        public Byte? NatureId { get; set; }
        public string NatureName { get; set; }

        public Byte? CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string OrganizationShortName { get; set; }

        public Byte? OrganizationTypeId { get; set; }
        public string OrganizationTypeName { get; set; }

        public long LotId { get; set; }
        public int LotNumber { get; set; }
        public decimal LotSum { get; set; }
        public string LotSourceOfFinancing { get; set; }
        public string LotStatus { get; set; }

        public string ContractStatus { get; set; }
        public string ContractUrl { get; set; }

        [Key]
        public long ObjectReadyId { get; set; }
        public string ObjectReadyName { get; set; }
        public string ObjectReadyUnit { get; set; }
        public decimal ObjectReadyAmount { get; set; }
        public decimal? ObjectReadyPrice { get; set; }
        public decimal? ObjectReadySum { get; set; }
        public decimal? ObjectReadyAmountCorrected { get; set; }
        public decimal? ObjectReadyPriceCorrected { get; set; }
        public decimal? ObjectReadySumCorrected { get; set; }
        public bool VNC { get; set; }
        public decimal? PriceClassifier { get; set; }
        public bool IsVed { get; set; }

        public decimal? protocolPrice { get; set; }

        public long? ReceiverId { get; set; }
        public string ReceiverShortName { get; set; }
        public Byte? ReceiverTypeId { get; set; }
        public string ReceiverTypeName { get; set; }

        public long? ObjectCalculatedId { get; set; }
        public decimal? ObjectCalculatedAmount { get; set; }
        public decimal? ObjectCalculatedPrice { get; set; }
        public decimal? ObjectCalculatedSum { get; set; }
        public decimal? ObjectCalculatedCoefficient { get; set; }
        public decimal? ObjectCalculatedRecoveredPrice { get; set; }
        public string RecoveryType { get; set; }
        public string RecoveryTypeShortName { get; set; }

        public long? ClassifierId { get; set; }
        public int? DrugId { get; set; }
        public string DrugTradeName { get; set; }
        public string DrugDescription { get; set; }
        public string GoodsCategoryName { get; set; }
        public int? DrugConsumerPackingCount { get; set; }
        public string DosageGroupDescription { get; set; }
        public int? OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public int? PackerId { get; set; }
        public string Packer { get; set; }

        public string ProvisorAction { get; set; }
        public string InnGroup { get; set; }

        public long? RegionId { get; set; }
        public string RegionFederalDistrict { get; set; }
        public string RegionFederationSubject { get; set; }
        public string RegionDistrict { get; set; }
        public string RegionCity { get; set; }
        public string RegionCode { get; set; }
        public bool UseContractData { get; set; }
        public string ReestrNumber { get; set; }
        public decimal? Contract_Sum { get; set; }
        public string ObjectType { get; set; }
        public string ObjectTypeRU { get; set; }
        public decimal? FDAveragePrice { get; set; }
        public decimal? FDCoefficient { get; set; }

        public decimal? kof_otkl { get; set; }
        public decimal? kofPriceGZotkl { get; set; }
    }
}
