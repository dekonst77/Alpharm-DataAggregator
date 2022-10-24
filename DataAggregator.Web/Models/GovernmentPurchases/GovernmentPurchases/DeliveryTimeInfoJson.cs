using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Web.Models.Common;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class DeliveryTimeInfoJson
    {
        public DeliveryTimeInfoJson()
        {
            
        }

        public DeliveryTimeInfoJson(DeliveryTimeInfo deliveryTimeInfo)
        {
            if (deliveryTimeInfo == null)
            {
                throw new ArgumentNullException("deliveryTimeInfo");
            }
            Id = deliveryTimeInfo.Id;
            DateStart = deliveryTimeInfo.DateStart;
            DateEnd = deliveryTimeInfo.DateEnd;
            Count = deliveryTimeInfo.Count;

            DeliveryTimePeriod = deliveryTimeInfo.DeliveryTimePeriod == null
                ? null
                : new DictionaryElementJsonByte() { Id = deliveryTimeInfo.DeliveryTimePeriod.Id, Name = deliveryTimeInfo.DeliveryTimePeriod.Name };
        }

        public int? Id { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public int? Count { get; set; }

        public DictionaryElementJsonByte DeliveryTimePeriod { get; set; }
    }
}