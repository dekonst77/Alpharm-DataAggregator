using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class ObjectJson
    {
        public ObjectJson(PurchaseObject purchaseObject)
        {
            if (purchaseObject == null)
            {
                throw new ArgumentNullException("purchaseObject");
            }

            Id = purchaseObject.Id;
            Name = purchaseObject.Name;
            OKPD = purchaseObject.OKPD;
            Unit = purchaseObject.Unit;
            Amount = purchaseObject.Amount;
            Price = purchaseObject.Price;
            Sum = purchaseObject.Sum;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string OKPD { get; set; }

        public string Unit { get; set; }

        public decimal Amount { get; set; }

        public decimal Price { get; set; }

        public decimal Sum { get; set; }
    }
}