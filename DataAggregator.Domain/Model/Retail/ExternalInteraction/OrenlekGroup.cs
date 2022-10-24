using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace DataAggregator.Domain.Model.Retail.ExternalInteraction
{
    [Table("OrenlekGroup", Schema = "External")]
    public class OrenlekGroup
    {
        public Guid Id { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public long TradeNameId { get; set; }

        public long FormProductId { get; set; }

        public long DosageGroupId { get; set; }

        public decimal Count { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal SellingSum { get; set; }

        public decimal PurchasePrice { get; set; }

        public decimal PurchaseSum { get; set; }

        public decimal PharmacyId { get; set; }

        public string RegionCode { get; set; }
    }
}
