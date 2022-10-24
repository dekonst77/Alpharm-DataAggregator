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
    public class SupplierJson
    {
        public SupplierJson()
        {
        }

        public SupplierJson(GovernmentPurchasesContext context, Supplier supplier)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (supplier == null)
            {
                throw new ArgumentNullException("supplier");
            }

            Id = supplier.Id;
            Name = supplier.Name;
            INN = supplier.INN;
            KPP = supplier.KPP;
            LocationAddress = supplier.LocationAddress;
            ContactMail = supplier.ContactMail;
            PhoneNumber = supplier.PhoneNumber;
        }

        public long Id { get; set; }

        public string Name { get; set; }

        public string INN { get; set; }

        public string KPP { get; set; }

        public string LocationAddress { get; set; }

        public string ContactMail { get; set; }

        public string PhoneNumber { get; set; }

        public Supplier ToDomain()
        {
            var result = new Supplier()
            {
                Id = Id,
                Name = Name,
                INN = INN,
                KPP = KPP,
                LocationAddress = LocationAddress,
                ContactMail = ContactMail,
                PhoneNumber = PhoneNumber
            };
            return result;
        }

    }
}