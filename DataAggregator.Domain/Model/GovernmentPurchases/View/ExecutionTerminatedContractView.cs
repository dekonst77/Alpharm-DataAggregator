using System;
using System.ComponentModel.DataAnnotations;
using DataAggregator.Domain.Model.DataReport;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    [Table("ExecutionTerminatedContractView", Schema = "Report")]
    public class ExecutionTerminatedContractView
    {
        [Key]
        [Column(Order = 0)]
        public long PurchaseId { get; set; }
        public string Number { get; set; }
        [Key]
        [Column(Order = 1)]
        public string ReestrNumber { get; set; }
        public string ContractNumber { get; set; }
        public string Url { get; set; }
        public decimal Sum { get; set; }
        public DateTime DateBegin { get; set; }
        public string Comment { get; set; }
    }
}
