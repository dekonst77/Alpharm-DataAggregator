using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    [Table("SelectionPurchaseLinkView", Schema = "search")]
    public class SelectionPurchaseLinkView
    {

        [Key]
        public long Id { get; set; }
        public int FzNumber { get; set; }
        public string Number { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Customer { get; set; }
        public string Method { get; set; }
        public string Stage { get; set; }
        public decimal? Sum { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? SearchDate { get; set; }
        public Byte? PurchaseClassId { get; set; }       
        public string PurchaseClassUser { get; set; }
        public DateTime? PurchaseClassDate { get; set; }
        public string PurchaseClass { get; set; }
        public string PurchaseStage { get; set; }
        public string ErrorMessage { get; set; }

    }
}
