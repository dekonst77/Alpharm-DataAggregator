using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAggregator.Domain.Model.ExternalDataRetail.external
{
    [Table("PharmacySellingStructure", Schema = "external")]
    public class PharmacySellingStructure
    {
        [Key]
        public long PharmacyId { get; set; }

        public string RegionCode { get; set; }

        public decimal? SellingSum { get; set; }

        public decimal? RxSum { get; set; }


        public decimal? OtcSum { get; set; }


        public decimal? BadSum { get; set; }


        public decimal? OtherSum { get; set; }
    }
}