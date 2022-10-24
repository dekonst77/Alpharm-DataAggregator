using DataAggregator.Domain.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.DrugClassifier.Classifier
{
    public class NFC : HierarchicalDictionaryItem<NFC>
    {
        public System.Int16? RouteAdministrationId { get; set; }
    }
    [Table("RouteAdministration", Schema = "Classifier")]
    public class RouteAdministration
    {
        [Key]
        public System.Int16 Id { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
    [Table("DDD_Norma", Schema = "Classifier")]
    public class DDD_Norma
    {
        [Key,Column(Order = 10)]
        public long ATCWhoId { get; set; }
        [Key, Column(Order = 20)]
        public System.Int16 RouteAdministrationId { get; set; }
        public System.Single DDD { get; set; }
        public string Units { get; set; }
        public string Description { get; set; }

        //[ForeignKey("RouteAdministrationId")]
        //public virtual RouteAdministration RouteAdministration { get; set; }

        //[ForeignKey("Units")]
        //public virtual DDD_Units DDD_Units { get; set; }

        //[ForeignKey("ATCWhoId")]
        //public virtual ATCWho ATCWho { get; set; }
    }
    [Table("DDD_Units", Schema = "Classifier")]
    public class DDD_Units
    {
        [Key]
        public string Id { get; set; }
        public string Value { get; set; }
    }
    [Table("EI", Schema = "Classifier")]
    public class EI
    {
        [Key]
        public Byte Id { get; set; }
        public string Value { get; set; }
    }
    [Table("DDD_Units_Standart", Schema = "Classifier")]
    public class DDD_Units_Standart
    {
        [Key]
        public string Id { get; set; }
        public decimal Value { get; set; }
    }
    [Table("DDDView", Schema = "Classifier")]
    public class DDDView
    {
        [Key]
        public long ClassifierId { get; set; }
        public long DrugId { get; set; }
        public long OwnerTradeMarkId { get; set; }
        public long PackerId { get; set; }


        public string RCNumber { get; set; }
        public string TradeName { get; set; }
        public string INNGroup { get; set; }
        public string FormProduct { get; set; }
        public string nfc_Value { get; set; }
        public string nfc_Description { get; set; }
        public string RouteAdministration { get; set; }
        public string who_Value { get; set; }
        public string who_Description { get; set; }

        public string main_Dos_In_Count { get; set; }
        public string main_Dos_In_Unit { get; set; }
        public string main_Dos_Total_Count { get; set; }
        public string main_Dos_Total_Unit { get; set; }
        public int count_INN { get; set; }
        public string inn_1 { get; set; }
        public string inn_1_Count { get; set; }
        public string inn_1_Unit { get; set; }
        public string inn_2 { get; set; }
        public string inn_2_Count { get; set; }
        public string inn_2_Unit { get; set; }
        public int ConsumerPackingCount { get; set; }

        public bool DDD_chek { get; set; }
        public System.Single DDD_Norma { get; set; }
        public string DDD_Units { get; set; }
        public string DDD_Comment { get; set; }
        public string DDD_Formula { get; set; }
        public decimal DDDs { get; set; }
        public void IsNull()
        {
            if (DDD_Units == null) DDD_Units = "";
            if (DDD_Comment == null) DDD_Comment = "";
            if (DDD_Formula == null) DDD_Formula = "";
        }
    }

    [Table("StandardUnitsView", Schema = "Classifier")]
    public class StandardUnitsView
    {
        [Key]
        public long ClassifierId { get; set; }
        public long DrugId { get; set; }
        public string RCNumber { get; set; }
        public string TradeName { get; set; }
        public string INNGroup { get; set; }
        public string FormProduct { get; set; }
        public string DosageGroup { get; set; }
        public int ConsumerPackingCount { get; set; }
        public string inn_1_Count { get; set; }
        public string inn_1_Unit { get; set; }
        public string inn_2_Count { get; set; }
        public string inn_2_Unit { get; set; }
        public string inn_3_Count { get; set; }
        public string inn_3_Unit { get; set; }
        public string main_Dos_In_Count { get; set; }
        public string main_Dos_In_Unit { get; set; }
        public string main_Dos_Total_Count { get; set; }
        public string main_Dos_Total_Unit { get; set; }
        public Byte EIId { get; set; }
        public string EI { get; set; }
        public decimal StandardUnits { get; set; }
        public decimal StandardUnits_Hand { get; set; }
        public bool StandardUnits_Ckeck { get; set; }
        public void IsNull()
        {

        }
    }
}