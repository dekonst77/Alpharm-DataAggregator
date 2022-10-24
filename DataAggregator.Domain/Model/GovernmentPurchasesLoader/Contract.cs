using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

    [Table("Contract", Schema = "buffer")]
    public class Contract
    {
        public long Id { get; set; }
        public string ReestrNumber { get; set; }
        public string Url { get; set; }
        public long? ContractStatusId { get; set; }
        public string ContractStatus { get; set; }
        public long OrganizationGZId { get; set; }
        public string OrganizationUrl { get; set; }
        public DateTime? ConclusionDate { get; set; }
        public string ContractNumber { get; set; }
        public long? MethodPriceId { get; set; }
        public string MethodPrice { get; set; }
        public decimal Sum { get; set; }
        public string Currency { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public long? SupplierId { get; set; }
        public string PurchaseNumber { get; set; }
        public decimal? ActuallyPaid { get; set; }
        public string TerminationBase { get; set; }
        public string TerminationDate { get; set; }
        public string TerminationReason { get; set; }
        public string BasisConcluding { get; set; }
        //1 - много поставщиков
        public int Error { get; set; }


        public virtual Supplier Supplier { get; set; }
        
    }
}