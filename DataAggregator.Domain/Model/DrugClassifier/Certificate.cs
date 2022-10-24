using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data.SqlTypes;


namespace DataAggregator.Domain.Model.DrugClassifier.Certificate
{
    [Table("Certificate", Schema = "grls")]
    public class Certificate
    {
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public string Number_ID { get; set; }
        public string type { get; set; }
        //public string html { get; set; }
        public bool Exchangeable { get; set; }
        public bool Reference { get; set; }
        public DateTime? data_end { get; set; }
        public DateTime? data_Annul { get; set; }
        public string StorageLife { get; set; }
        public DateTime? date_registration { get; set; }
        public string Owner_Name { get; set; }
        public string Owner_Country { get; set; }
        public string TN { get; set; }
        public string INN { get; set; }
        //public string FV_raw { get; set; }
        //public string ManfWay_raw { get; set; }
        public string FTG { get; set; }
        // public string SubstRaw { get; set; }
        public string ATC_WHO { get; set; }
        public bool ved { get; set; }
        public DateTime? last_update { get; set; }
        public DateTime? last_control { get; set; }
        public string status { get; set; }
    }

    [Table("FV", Schema = "grls")]
    public class FV
    {
        [Key]
        public int Id { get; set; }
        public int CertificateId { get; set; }
        [ForeignKey("CertificateId")]
        public virtual Certificate Certificate { get; set; }
        public string LekForm { get; set; }
        public string Dosage { get; set; }
        public string ExpirationDate { get; set; }
        public string StorageCondition { get; set; }
        public string FormV { get; set; }
    }
    [Table("ManufactureWay", Schema = "grls")]
    public class ManufactureWay
    {
        [Key]
        public int Id { get; set; }
        public int CertificateId { get; set; }
        [ForeignKey("CertificateId")]
        public virtual Certificate Certificate { get; set; }


        public int Np { get; set; }
        public string Stage { get; set; }
        public string Manufacturer { get; set; }
        public long? PackerId { get; set; }
        [ForeignKey("PackerId")]
        public virtual Classifier.Manufacturer PackerSpr { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public long? ManufacturerClearId { get; set; }
        [ForeignKey("ManufacturerClearId")]
        public virtual Classifier.ManufacturerClear ManufacturerClearSpr { get; set; }

        public System.Byte Status { get; set; }
    }

    [Table("ManufactureWayView", Schema = "grls")]
    public class ManufactureWayView
    {
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public string Number_ID { get; set; }
        public string CertStatus { get; set; }
        public string CertType { get; set; }
        public string TN { get; set; }
        public string INN { get; set; }
        public int Np { get; set; }
        public string Stage { get; set; }
        public string Manufacturer { get; set; }
        public long? ManufacturerClearId { get; set; }
        public string ManufacturerClearValue { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public long? PackerId { get; set; }
        public string PackerValue { get; set; }
        public System.Byte Status { get; set; }
    }

    [Table("SubstRaw", Schema = "grls")]
    public class SubstRaw
    {
        [Key]
        public int Id { get; set; }
        public int CertificateId { get; set; }
        [ForeignKey("CertificateId")]
        public virtual Certificate Certificate { get; set; }


        public string INN { get; set; }
        public string INN2 { get; set; }
        public string Manufacturer { get; set; }
        public long? ManufacturerId { get; set; }
        [ForeignKey("ManufacturerId")]
        public virtual Classifier.Manufacturer ManufacturerSpr { get; set; }
        public string Address { get; set; }
        public string ExpirationDate { get; set; }
        public string StorageCondition { get; set; }
        public string NumberND { get; set; }
        public string Nark { get; set; }

    }
    [Table("NumberINN", Schema = "grls")]
    public class NumberINN
    {
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public string INN { get; set; }
        public string LekForm { get; set; }
        public int Id_new { get; set; }
    }
    [Table("SubstanceView", Schema = "grls")]
    public class SubstanceView
    {
        [Key]
        public int Id { get; set; }
        public int Id_new { get; set; }
        public string Number { get; set; }
        public string INN { get; set; }
        public string LekForm { get; set; }
        public bool isMain { get; set; }
        public int Chemicals_Count { get; set; }
        public int Classifier_Count { get; set; }
    }
    [Table("ChemicalSPR", Schema = "grls")]
    public class ChemicalSPR
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
    [Table("Chemicals", Schema = "grls")]
    public class Chemicals
    {
        [Key]
        public int Id { get; set; }
        public int NumberINN_NewId { get; set; }
        public int ChemicalSPRId { get; set; }

        [ForeignKey("NumberINN_NewId")]
        public virtual NumberINN NumberINN { get; set; }

        [ForeignKey("ChemicalSPRId")]
        public virtual ChemicalSPR ChemicalSPR { get; set; }
    }

    [Table("ChemicalsView", Schema = "grls")]
    public class ChemicalsView
    {
        [Key]
        public int Id { get; set; }
        public int NumberINN_NewId { get; set; }
        public string INN { get; set; }
        public string LekForm { get; set; }
        public int ChemicalSPRId { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }

    }
    [Table("Flag", Schema = "grls")]
    public class Flag
    {
        [Key]
        public byte Id { get; set; }
        public string Value { get; set; }
    }
        [Table("ESKLP", Schema = "grls")]
    public class ESKLP
    {
        [Key]
        public string Id { get; set; }
        public DateTime last_update { get; set; }
        public string Id2 { get; set; }
        public string StandardINN { get; set; }
        public string StandardFV { get; set; }
        public string TradeName { get; set; }
        public string NormINN { get; set; }
        public string NormFV { get; set; }
        public string NormDosage { get; set; }
        public string EI { get; set; }
        public double AmountEISecond { get; set; }
        public double FirstAmount { get; set; }
        public string FirstFV { get; set; }
        public double SecondAmount { get; set; }
        public string SecondFV { get; set; }
        public string SecondFVAdd { get; set; }
        public string RuNumber { get; set; }
        public string RuOwner { get; set; }
        public string RuCountry { get; set; }
        public DateTime? RuDate { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerCountry { get; set; }
        public string ManufacturerAddress { get; set; }
        public bool IsVed { get; set; }
        public bool IsNark { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public DateTime? DateMod { get; set; }
        public string MaxPrice { get; set; }
        public string AddText { get; set; }
        public long? ClassifierId { get; set; }
        public int? ClassifierPackingId { get; set; }
        public bool IsActual { get; set; }
        public bool IsGo { get; set; }
        public byte Flag { get; set; }
        [ForeignKey("Flag")]
        public virtual Flag FlagSpr { get; set; }
    }
    [Table("ESKLPView", Schema = "grls")]
    public class ESKLPView:ESKLP
    {
    }
}
