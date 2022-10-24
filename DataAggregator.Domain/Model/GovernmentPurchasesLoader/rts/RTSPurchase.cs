using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader.rts
{
    [Table("Purchase", Schema = "rts")]
    public class RTSPurchase
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public long CustomerId { get; set; }
        public string Name { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateEndFirstParts { get; set; }
        public string FilingPlace { get; set; }
        public string Url { get; set; }
        public string SiteName { get; set; }
        public string SiteUrl { get; set; }
        public Byte StageId { get; set; }
        public string DeliveryTime { get; set; }
        public decimal? Sum { get; set; }
        public int StatusId { get; set; }

        public virtual RTSCustomer Customer { get; set; }
    }
}
