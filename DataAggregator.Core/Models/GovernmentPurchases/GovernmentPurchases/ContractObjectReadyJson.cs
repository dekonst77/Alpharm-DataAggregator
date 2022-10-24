using System;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Core.Models.GovernmentPurchases.GovernmentPurchases
{
    public class ContractObjectReadyJson
    {
        public ContractObjectReadyJson(ContractObjectReady contractObjectReady)
        {
            if (contractObjectReady == null)
            {
                throw new ArgumentNullException("contractObjectReady");
            }

            Id = contractObjectReady.Id;
            Name = contractObjectReady.Name;
            Unit = contractObjectReady.Unit;
            Amount = contractObjectReady.Amount;
            Price = contractObjectReady.Price;
            Sum = contractObjectReady.Sum;
        }

        public ContractObjectReadyJson()
        {
            
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Unit { get; set; }

        public decimal Amount { get; set; }

        public decimal? Price { get; set; }

        public decimal? Sum { get; set; }

        public void CopyTo(ContractObjectReady contractObjectReady)
        {
            contractObjectReady.Amount = this.Amount;
            contractObjectReady.Unit = this.Unit;
            contractObjectReady.Price = this.Price;
            contractObjectReady.Sum = this.Sum;

            var name = ClearDoubleSpace(this.Name.Trim());

            contractObjectReady.Name = name;
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