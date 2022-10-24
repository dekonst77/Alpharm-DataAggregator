using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.GovernmentPurchases;
using DataAggregator.Domain.Model.GovernmentPurchases.View;
using DataAggregator.Domain.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DataAggregator.Web.Models.GovernmentPurchases.GovernmentPurchases
{
    public class PurchaseLightJson
    {
        public PurchaseLightJson()
        {
        }

        public PurchaseLightJson(Purchase purchase)
        {
            if (purchase == null)
            {
                throw new ArgumentNullException("purchase");
            }

            Id = purchase.Id;
            Number = purchase.Number;
            Name = purchase.Name;
            URL = purchase.URL;
            if (purchase.IsPriceByPart == true)
                IsPriceByPart = true;
            else
                IsPriceByPart = false;
        }

        public long Id { get; set; }

        public string Number { get; set; }
        public string URL { get; set; }

        public string Name { get; set; }

        public bool IsPriceByPart { get; set; }
    }
}