using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
namespace DataAggregator.Domain.Model.OFD
{
    [Table("Log", Schema = "dbo")]
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public string param_s { get; set; }
        public long param_i { get; set; }
        public DateTime dt { get; set; }
        public bool send_rep { get; set; }
    }
    [Table("ofdFilenames_View", Schema = "dbo")]
    public class ofdFilenames_View
    {
        [Key]
        public int Id { get; set; }
        public string value { get; set; }        
        public DateTime dt_load { get; set; }
        public DateTime period { get; set; }
        public Int16 ActionType { get; set; }
        public Int16 SupplierId { get; set; }
        public string ActionName { get; set; }
        public string SupplierName { get; set; }
        public string SupplierNameNapr { get; set; }
    }
    [Table("Aggregated_Period", Schema = "dbo")]
    public class Aggregated_Period
    {
        [Key]
        public int Id { get; set; }
        public Int16 SupplierId { get; set; }
        public DateTime period { get; set; }
        public byte period_type { get; set; }

        public decimal sum_10 { get; set; }
        public decimal sum_20 { get; set; }
        public decimal sum_30 { get; set; }

        public decimal amount_10 { get; set; }
        public decimal amount_20 { get; set; }
        public decimal amount_30 { get; set; }

        public int count_brik_10 { get; set; }
        public int count_brik_20 { get; set; }
        public int count_brik_30 { get; set; }

        public int count_tran_10 { get; set; }
        public int count_tran_20 { get; set; }
        public int count_tran_30 { get; set; }
    }
    [Table("OwnerAgr", Schema = "4SC")]
    public class OwnerAgr
    {
        [Key]
        public int OwnerAgrId { get; set; }
        public string Value { get; set; }
    }
        [Table("Period", Schema = "4SC")]
    public class Period_4SC
    {
        [Key]
        public int Id { get; set; }
        public Int16 SupplierId { get; set; }
        public int OwnerAgrId { get; set; }
        public DateTime period { get; set; }
        public byte period_type { get; set; }

        public decimal sum_10 { get; set; }
        public decimal sum_20 { get; set; }
        public decimal sum_30 { get; set; }

        public decimal amount_10 { get; set; }
        public decimal amount_20 { get; set; }
        public decimal amount_30 { get; set; }

        public int count_brik_10 { get; set; }
        public int count_brik_20 { get; set; }
        public int count_brik_30 { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }
        
       [ForeignKey("OwnerAgrId")]
       public virtual OwnerAgr OwnerAgr { get; set; }
    }
    [Table("Week_Periods", Schema = "dbo")]
    public class Week_Periods
    {
        [Key]
        public int Id { get; set; }
        public Int16 SupplierId { get; set; }
        public int period_wk { get; set; }
        public DateTime dt_start { get; set; }
        public DateTime dt_end { get; set; }
        public byte period_type { get; set; }


        public decimal amount_15 { get; set; }
        public decimal amount_20 { get; set; }
        public decimal amount_30 { get; set; }

        public int count_brik_15 { get; set; }
        public int count_brik_20 { get; set; }
        public int count_brik_30 { get; set; }
    }
    [Table("List", Schema = "report")]
    public class List
    {
        [Key]
        public int Id { get; set; }
        public string value { get; set; }
        public string cmd { get; set; }
    }
    [Table("Supplier", Schema = "dbo")]
    public class Supplier
    {
        [Key]
        public Int16 Id { get; set; }
        public string value { get; set; }
        public string folder { get; set; }
        public string GS_to_OFD_select { get; set; }
    }
    [Table("Aggregated_All", Schema = "dbo")]
    public class Aggregated_All
    {
        [Key]
        public long Id { get; set; }
        public int FilenameId { get; set; }
        public System.Int16 SupplierId { get; set; }
        public long classifier_id_psevdo { get; set; }
        public int BrickId { get; set; }
        public decimal amount { get; set; }
        public decimal summa { get; set; }
        public DateTime period { get; set; }
        public decimal? amount_calc { get; set; }
        public long? ClassifierId { get; set; }
        public int? Periodw { get; set; }
        public System.Byte? period_type { get; set; }
        public double? Min_price { get; set; }
        public double? Max_price { get; set; }
        public double? Avg_price { get; set; }
        public double? Median_price { get; set; }
        public double? Mode_price { get; set; }
    }
    [Table("Brick_L3_all", Schema = "dbo")]
    public class Brick_L3_all
    {
        [Key]
        public string Id { get; set; }
        public string L1_label { get; set; }
        public string L2_label { get; set; }
        public string L3_label { get; set; }
    }
    [Table("Data_All", Schema = "4SC")]
    public class Data_All_4SC
    {
        [Key]
        public long Id { get; set; }
        public int FilenameId { get; set; }
        public System.Int16 SupplierId { get; set; }
        public int? AgreementId { get; set; }
        public DateTime period { get; set; }
        public int PharmacyId { get; set; }
        public int OwnerId { get; set; }
        public string ReceiptId { get; set; }
        public string stringid { get; set; }
        public string checkitem { get; set; }
        public DateTime DateTime { get; set; }
        public long classifier_id_psevdo { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SellingCount { get; set; }
        public decimal SellingSum { get; set; }
        public long ClassifierId { get; set; }
        public System.Byte period_type { get; set; }
        public long? ClassifierId_korr { get; set; }
        public long? ClassifierId_hand { get; set; }
        public decimal? SellingCountCorr { get; set; }


    }

}

