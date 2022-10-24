using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataAggregator.Web.Models.GovernmentPurchases.Suppliers
{
    public class SupplierRawFilterJson
    {
        public bool Ready { get; set; }

        public bool NotReady { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string INN { get; set; }

        public string KPP { get; set; }

        public string SupplierId { get; set; }

        public string SupplierName { get; set; }
    }
}