namespace DataAggregator.Domain.Model.Retail
{
    public class AggregatedRawDataByClassifier
    {
        public int? Year { get; set; }
        public int? Month { get; set; }

        public string SourceName{ get; set; }
        public long? FileInfoId{ get; set; }
        public string Path{ get; set; }
        public long? TargetPharmacyId{ get; set; }

        public string RegionName { get; set; }

        public decimal? PurchaseSumNds { get; set; }
        public decimal? SellingSumNds { get; set; }
        public decimal? PurchaseCount { get; set; }
        public decimal? SellingCount { get; set; }
        public decimal? PurchasePriceNds { get; set; }
        public decimal? SellingPriceNds { get; set; }

        public int IsTpBlackList { get; set; }
        public int IsTpBrandBlackList { get; set; }
        public int IsSprBlackList { get; set; }

        public string Name { get; set; }
    }
}