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

namespace DataAggregator.Web.Models.GovernmentPurchases.Suppliers
{
    public class SupplierRawBindingJson
    {
        public SupplierRawBindingJson()
        {
        }

        public SupplierRawBindingJson(GovernmentPurchasesContext context, SupplierRawBinding supplierRawBinding)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (supplierRawBinding == null)
            {
                throw new ArgumentNullException("supplierRawBinding");
            }

            Id = supplierRawBinding.Id;
            Name = supplierRawBinding.Name;
            Country = supplierRawBinding.Country;
            CountryCode = supplierRawBinding.CountryCode;
            LocalAddress = supplierRawBinding.LocalAddres;
            Address = supplierRawBinding.Addres;
            Phone = supplierRawBinding.Phone;
            Email = supplierRawBinding.Email;
            Status = supplierRawBinding.Status;
            OKPO = supplierRawBinding.OKPO;
            INN = supplierRawBinding.INN;
            KPP = supplierRawBinding.KPP;
            KPPDate = supplierRawBinding.KPPDate;
            SupplierId = supplierRawBinding.SupplierId;
            SupplierName = supplierRawBinding.SupplierName;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public int? CountryCode { get; set; }

        public string LocalAddress { get; set; }

        public string Address { get; set; }

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