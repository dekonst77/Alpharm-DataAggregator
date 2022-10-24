using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class ShipmentInfo
    {
        [Key]
        public long PurchaseId { get; set; }

        public bool FromPurchase { get; set; }

        public bool FromContract { get; set; }
    }
}
