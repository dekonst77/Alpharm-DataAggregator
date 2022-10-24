using System;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Core.Models.GovernmentPurchases.GovernmentPurchases
{
    public class ObjectReadyJson
    {
        public ObjectReadyJson(PurchaseObjectReady purchaseObjectReady)
        {
            if (purchaseObjectReady == null)
            {
                throw new ArgumentNullException("purchaseObjectReady");
            }

            Id = purchaseObjectReady.Id;
            Name = purchaseObjectReady.Name;
            Unit = purchaseObjectReady.Unit;
            Amount = purchaseObjectReady.Amount;
            Price = purchaseObjectReady.Price;
            Sum = purchaseObjectReady.Sum;
        }

        public ObjectReadyJson()
        {
            
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public decimal Amount { get; set; }

        public decimal Price { get; set; }

        public decimal Sum { get; set; }

        public void CopyTo(PurchaseObjectReady purchaseObjectReady)
        {
            purchaseObjectReady.Amount = this.Amount;
            purchaseObjectReady.Unit = this.Unit;
            purchaseObjectReady.Price = this.Price;
            purchaseObjectReady.Sum = this.Sum;
            purchaseObjectReady.Name = this.Name;
        }
    }
}