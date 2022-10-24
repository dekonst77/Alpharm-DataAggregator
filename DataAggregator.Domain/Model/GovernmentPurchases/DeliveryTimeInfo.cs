using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.GovernmentPurchases
{
    public class DeliveryTimeInfo
    {
        public int Id { get; set; }

        public long PurchaseId { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public int? Count { get; set; }

        public Byte DeliveryTimePeriodId { get; set; }

        public virtual Purchase Purchase { get; set; }

        public virtual DeliveryTimePeriod DeliveryTimePeriod { get; set; }

        public Guid? LastChangedUserIdDTI { get; set; }

        public DateTime? LastChangedDateDTI { get; set; }

    }
}
