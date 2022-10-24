using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class BionicaMediaReport
    {
        [Key, Column(Order = 1)]
        public long DrugId { get; set; }
        public long TradeNameId { get; set; }
        public string TradeName { get; set; }
        public long? INNGroupId { get; set; }
        public string INNGroup { get; set; }
        public bool? IsCompound { get; set; }
        public long? DosageGroupId { get; set; }
        public string DosageGroup { get; set; }
        public long FormProductId { get; set; }
        public string FormProduct { get; set; }
        public int ConsumerPackingCount { get; set; }
        [Key, Column(Order = 2)]
        public long OwnerTradeMarkId { get; set; }
        public string OwnerTradeMark { get; set; }
        public string OwnerTradeMarkCountry { get; set; }
        public long DrugTypeId { get; set; }
        public string DrugType { get; set; }
        public long? FTGId { get; set; }
        public string FTG { get; set; }
        public long? ATCWhoId { get; set; }
        public string ATCWhoCode { get; set; }
        public string ATCWhoDescription { get; set; }
        public long? ATCEphmraId { get; set; }
        public string ATCEphmraCode { get; set; }
        public string ATCEphmraDescription { get; set; }
        public long? ATCBaaId { get; set; }
        public string ATCBaaCode { get; set; }
        public string ATCBaaDescription { get; set; }
        public bool IsOtc { get; set; }
        public bool IsRx { get; set; }
        [Key, Column(Order = 3)]
        public long PackerId { get; set; }
        public string Packer { get; set; }
        public string PackerCountry { get; set; }
        public string DrugDescription { get; set; }
        public long? BrandId { get; set; }
        public string Brand { get; set; }
        public long? CorporationId { get; set; }
        public string Corporation { get; set; }
        public long? OwnerRegistrationCertificateId { get; set; }
        public string OwnerRegistrationCertificate { get; set; }
        public string OwnerRegistrationCertificateCountry { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? RegistrationDate { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? ReissueDate { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime? ExpDate { get; set; }
        public string CirculationPeriod { get; set; }
        public bool Used { get; set; }
        public int? VED2015 { get; set; }
        public int? VED2016 { get; set; }
        public int? VED2017 { get; set; }
        public int? FB2015 { get; set; }
        public int? FB2016 { get; set; }
        public int? FB2017 { get; set; }
        public bool MinimumAssortment { get; set; }
        public int? FB2018 { get; set; }
        public bool Exchangeable { get; set; }
        public bool Reference { get; set; }
        public decimal? SellingPrice { get; set; }
        public long? RegistrationCertificateId { get; set; }
        public int? VED2018 { get; set; }
        public int? FB2014 { get; set; }
        public long? NfcId { get; set; }
        public string NfcCode { get; set; }
        public string NfcDescription { get; set; }
    }
}