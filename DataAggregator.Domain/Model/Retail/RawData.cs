using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAggregator.Domain.Model.Retail
{
    public class RawData
    {
        public long Id { get; set; }

        public string Drug { get; set; }

        public string Barcode { get; set; }

        public string Manufacturer { get; set; }

        public decimal? PurchasePrice { get; set; }

        public decimal? SellingPrice { get; set; }

        //public decimal? Count { get; set; }

        //public string InternalCode { get; set; }

        //public long PharmacyId { get; set; }

        public long? DataId { get; set; }

        public virtual Data Data { get; set; }

        //public virtual Pharmacy Pharmacy { get; set; }

        public long? SourcePharmacyId { get; set; }

        public virtual SourcePharmacy SourcePharmacy { get; set; }
    }
}
