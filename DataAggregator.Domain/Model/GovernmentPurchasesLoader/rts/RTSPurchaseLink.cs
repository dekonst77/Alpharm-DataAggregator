using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.rts
{
    [Table("PurchaseLink", Schema = "rts")]
    public class RTSPurchaseLink
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public long SearchTaskId { get; set; }
        public string Url { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
