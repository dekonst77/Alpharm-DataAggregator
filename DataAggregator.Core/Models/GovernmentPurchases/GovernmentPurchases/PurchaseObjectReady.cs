using System;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Core.Models.GovernmentPurchases.GovernmentPurchases
{
    public class PurchaseObjectReadyJson
    {
        public PurchaseObjectReadyJson(PurchaseObjectReady purchaseObjectReady)
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
            if (purchaseObjectReady.Receiver != null)
            {
                ReceiverShortName = purchaseObjectReady.Receiver.ShortName;
                ReceiverId = purchaseObjectReady.Receiver.Id;
            }
        }

        public PurchaseObjectReadyJson()
        {
            
        }

        public string ReceiverShortName { get; set; }

        public long? ReceiverId { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public decimal Amount { get; set; }

        public decimal? Price { get; set; }

        public decimal? Sum { get; set; }

        public void CopyTo(PurchaseObjectReady purchaseObjectReady)
        {
            purchaseObjectReady.Amount = this.Amount;
            purchaseObjectReady.Unit = this.Unit;
            purchaseObjectReady.Price = this.Price;
            purchaseObjectReady.Sum = this.Sum;

            var name = ClearDoubleSpace(this.Name.Trim());

            purchaseObjectReady.Name = name;
        }

        private string ClearDoubleSpace(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            var result = source.Replace("  ", " ");

            if (result == source)
            {
                return result;
            }

            return ClearDoubleSpace(result);
        }
    }
}