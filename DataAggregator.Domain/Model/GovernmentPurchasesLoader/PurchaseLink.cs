using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

    [Table("PurchaseLink", Schema = "search")]
    public class PurchaseLink
    {
        public long Id { get; set; }
        public long? SearchPageId { get; set; }
        public string Number { get; set; }
        public string Link { get; set; }
        public string Name { get; set; }
        public string Customer { get; set; }
        public int FzNumber { get; set; }
        public long? SearchINNTaskId { get; set; }
        public long? SearchTaskId { get; set; }
        public long? ContractSearchTaskId { get; set; }
        public string Method { get; set; }
        public string Stage { get; set; }
        public decimal? Sum { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool GeneratedFromContract { get; set; }
        

        public virtual SearchPage SearchPage { get; set; }
        
    }
}