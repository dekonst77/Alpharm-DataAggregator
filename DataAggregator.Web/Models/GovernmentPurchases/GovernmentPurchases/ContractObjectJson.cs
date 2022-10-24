using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.Model.GovernmentPurchases;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class ContractObjectJson
    {
        public ContractObjectJson(ContractObject contractObject)
        {
            if (contractObject == null)
            {
                throw new ArgumentNullException("contractObject");
            }

            Id = contractObject.Id;
            Name = contractObject.Name;
            OKPD = contractObject.OKPD;
            Unit = contractObject.Unit;
            Amount = contractObject.Amount;
            Price = contractObject.Price;
            Sum = contractObject.Sum;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string OKPD { get; set; }

        public string Unit { get; set; }

        public decimal? Amount { get; set; }

        public decimal? Price { get; set; }

        public decimal? Sum { get; set; }
    }
}