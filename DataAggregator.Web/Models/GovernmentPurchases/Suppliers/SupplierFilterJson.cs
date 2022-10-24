using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.GovernmentPurchases.Suppliers
{
    public class SupplierFilterJson
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string INN { get; set; }

        public string KPP { get; set; }

        public string LocationAddress { get; set; }

        public string ContactMail { get; set; }

        public string PhoneNumber { get; set; }
    }
}