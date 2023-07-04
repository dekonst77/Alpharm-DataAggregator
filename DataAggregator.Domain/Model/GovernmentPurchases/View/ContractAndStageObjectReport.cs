using DataAggregator.Domain.Model.DataReport;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class ContractAndStageObjectReport
    {
        [Key]
        [Column(Order = 0)]
        public int Ind { get; set; }
        [Key]
        [Column(Order = 1)]
        public long PurchaseId { get; set; }
        [Key]
        [Column(Order = 2)]
        public long ContractId { get; set; }
        public string Type { get; set; }
        public string Number { get; set; }
        public string ReestrNumber { get; set; }
        public decimal? ContractSum { get; set; }
        public decimal? ActuallyPaid { get; set; }
        public decimal? DiffSum { get; set; }
        public decimal? SumIsp { get; set; }
        [Key]
        [Column(Order = 3)]
        public long ObjectId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Price { get; set; }
        public decimal? Sum { get; set; }
        public long? ClassifierId { get; set; }
        public string INNGroup { get; set; }
        public string TradeName { get; set; }
        public string DrugDescription { get; set; }
        public string Corporation { get; set; }
        public string Packer { get; set; }
        public decimal? ObjectCalculatedAmount { get; set; }
        public decimal? ObjectCalculatedPrice { get; set; }
        public decimal? ObjectCalculatedSum { get; set; }
        public string Seria { get; set; }
    }
}
