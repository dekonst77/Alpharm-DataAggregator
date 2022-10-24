using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAggregator.Domain.Utils;

namespace DataAggregator.Domain.Model.Retail
{
    [Table("FileData", Schema = "dbo")]
    public class FileData
    {
        public long Id { get; set; }

        public string Drug { get; set; }

        public long FileInfoId { get; set; }


        public string Barcode { get; set; }

        public string Manufacturer { get; set; }


        public decimal? PurchasePrice { get; set; }


        public decimal? PurchasePriceNDS { get; set; }


        public decimal? PurchaseSum { get; set; }


        public decimal? PurchaseSumNDS { get; set; }


        public decimal? SellingPrice { get; set; }


        public decimal? SellingPriceNDS { get; set; }


        public decimal? SellingSum { get; set; }


        public decimal? SellingSumNDS { get; set; }


        public decimal? PurchaseCount { get; set; }


        public decimal? SellingCount { get; set; }

        public string PharmacyKey { get; set; }

        public string PharmacyAddress { get; set; }

        public string PharmacyName { get; set; }

        public long TemplateId { get; set; }

        public virtual Template Template { get; set; }

        public virtual FileInfo FileInfo { get; set; }
    }
}
