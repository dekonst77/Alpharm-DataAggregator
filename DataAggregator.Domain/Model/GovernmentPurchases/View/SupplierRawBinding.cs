using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataAggregator.Domain.Model.GovernmentPurchases.View
{
    public class SupplierRawBinding
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public int? CountryCode { get; set; }

        public string LocalAddres { get; set; }

        public string Addres { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public string OKPO { get; set; }

        public string INN { get; set; }

        public string KPP { get; set; }

        public DateTime? KPPDate { get; set; }

        public long? SupplierId { get; set; }

        public string SupplierName { get; set; }
    }
}
