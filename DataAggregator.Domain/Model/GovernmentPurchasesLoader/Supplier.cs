using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{
    [Table("Supplier", Schema = "buffer")]
    public class Supplier
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameIndex { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string LocalAddres { get; set; }
        public string Addres { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string OKPO { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public DateTime? KPPDate { get; set; }
    }
}