using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DataAggregator.Domain.Utils;
using System.ComponentModel.DataAnnotations;
namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("Contract_check_view", Schema = "dbo")]
    public class Contract_check_view
    {
        public long Id { get; set; }
        public string url_P { get; set; }
        public string ContractStatus { get; set; }
        public string url_C { get; set; }
        public string Number { get; set; }
        public string change_reason { get; set; }
        public string Supplier_Name { get; set; }
        public string Supplier_INN { get; set; }

        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime DateBegin { get; set; }
        public string ReestrNumber { get; set; } 
        public decimal? SUM_TZ { get; set; }
        public decimal? Sum{ get; set; }
        public decimal? Sum_new { get; set; }
        public decimal? Sum_delta { get; set; }
        //public decimal? ActuallyPaid_delta { get; set; }
        public decimal? Sum_new_ActuallyPaid { get; set; }
        public decimal? ActuallyPaid { get; set; }
       // public decimal? ActuallyPaid_new { get; set; }
        public string TZ { get; set; }
        public int COR_Count { get; set; }
        public string LastChangedObjectsUser { get; set; }
        public DateTime? LastChangedObjectsDate { get; set; }
        public string KKName { get; set; }
    }

    //[Table("Contract_check_ContractPaymentStage_view", Schema = "dbo")]
    public class Contract_check_ContractPaymentStage_view
    {
        [Key]
        public long ContractId { get; set; }
        public long PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverterWithTime))]
        public DateTime DateBegin { get; set; }
        public Byte NatureId { get; set; }
        public string NatureName { get; set; }
        public long LotId { get; set; }
        
        public string ReestrNumber { get; set; }
        public string FundingNames { get; set; }
        public string Url { get; set; }

        public string KBKs { get; set; }
        public string KBKs_new { get; set; }

        public decimal? KBK_sum { get; set; }
        public decimal? KBK_sum_new { get; set; }

        public string Status { get; set; }

        public string ReceiverFederationSubject { get; set; }
    }
}
