using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class NotExportedToExternalPurchasesView
    {
        [Key]
        public long PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public string PurchaseName { get; set; }
        public decimal? LotSum { get; set; }
        public DateTime PurchaseDateBegin { get; set; }
        public int BadCount { get; set; }
        public int BadLotSum { get; set; }
        public int BadObjects { get; set; }
        public int BadNature { get; set; }
        public int BadDeliveryTimeInfo { get; set; }
        public int BadLotFunding { get; set; }
        public int BadCoefficient { get; set; }
    }
}
