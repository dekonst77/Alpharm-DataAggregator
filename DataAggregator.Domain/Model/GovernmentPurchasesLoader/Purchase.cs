using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.GovernmentPurchasesLoader
{

    [Table("Purchase", Schema = "buffer")]
    public class Purchase
    {
        public Purchase()
        {
            this.Lot = new HashSet<Lot>();
        }

        public long Id { get; set; }
        public string Number { get; set; }
        public long? CustomerId { get; set; }
        public Byte LawTypeId { get; set; }
        public Byte MethodId { get; set; }
        public string SiteName { get; set; }
        public string SiteURL { get; set; }
        public string Name { get; set; }
        public Byte? StageId { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime? DateEndFirstParts { get; set; }
        public string FilingPlace { get; set; }
        public string URL { get; set; }
        //public string RegionCode { get; set; }
        //public long? RegionId { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreate { get; set; }
        public Byte PurchaseClassId { get; set; }
        public string DeliveryTime { get; set; }
        public string WhoIsPurchasing { get; set; }
        public string PriceJustification { get; set; }
        public string ConditionsForForeigners { get; set; }
        public Byte? CategoryId { get; set; }
        public Byte? NatureId { get; set; }
        public string PostIndex { get; set; }
        public long? OrganizationGZId { get; set; }
        public string OrganizationUrl { get; set; }
        public string PostAddress { get; set; }
        public int? MigrateStatusId { get; set; }

        public virtual ICollection<Lot> Lot { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime PublishDate { get; set; }
    }
}